using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ST
{
    public class SetupController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Transform ssdListContent;

        [SerializeField]
        private GameObject ssdListEntryPrefab;

        [SerializeField]
        private SetupInfoPanel infoPanel;

        [SerializeField]
        private Transform playerShipsListContent;

        [SerializeField]
        private GameObject shipsListEntryPrefab;

        [SerializeField]
        private TMP_Text totalCostText;

        [SerializeField]
        private string shipPrefabName;

        [SerializeField]
        private Button readyButton;

        [SerializeField]
        private Canvas dynamicCanvas;

        [SerializeField]
        private Canvas waitingCanvas;

        [SerializeField]
        private TMP_Text nbReadyText;

        [SerializeField]
        private ShipSSD[] availableShips;

        private void Start()
        {
            FillSSDList();

            infoPanel.OnAddShip.AddListener(AddShip);
            readyButton.onClick.AddListener(SetReady);
        }

        private void SetReady()
        {
            PhotonNetwork.LocalPlayer.SetReady();
        }

        private void FixedUpdate()
        {
            UpdateTotalCost();
            readyButton.gameObject.SetActive(PhotonNetwork.LocalPlayer.GetShips().Count > 0);
        }

        public override void OnPlayerPropertiesUpdate(Player player, Hashtable changedProps)
        {
            var ready = PhotonNetwork.LocalPlayer.IsReady();
            dynamicCanvas.gameObject.SetActive(!ready);
            waitingCanvas.gameObject.SetActive(ready);

            UpdateNbReady();
        }

        private void UpdateNbReady()
        {
            var nbReady = PhotonNetwork.CurrentRoom.Players.Values
                .Where(p => p.IsReady())
                .Count();

            nbReadyText.text = string.Format("{0} / {1} ready", nbReady, PhotonNetwork.CurrentRoom.PlayerCount);

            if (PhotonNetwork.IsMasterClient && nbReady == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                PhotonNetwork.CurrentRoom.ResetPlayersReadiness();
                PhotonNetwork.LoadLevel(GameSettings.ScenePlay); // TODO Load Deploy scene when available
            }
        }

        private void FillSSDList()
        {
            foreach (var ssd in availableShips)
            {
                var entry = Instantiate(ssdListEntryPrefab).GetComponent<SsdListEntry>();
                entry.SSD = ssd;
                entry.OnSelect.AddListener(() => { infoPanel.SSD = ssd; });
                entry.transform.SetParent(ssdListContent);
            }
        }

        private void AddShip(AddShipEventData data)
        {
            // Check if player ships cost + this ship cost do not exceed max points
            var maxPoints = PhotonNetwork.CurrentRoom.GetMaxPoints();
            var currentCost = GetPlayerShipsTotalCost();
            var addedCost = data.SSD.BaseCost;

            if (currentCost + addedCost <= maxPoints)
            {
                // Create the ship
                Debug.LogFormat("<color=green>Create ship</color> {0} [{1}]", data.ShipName, data.SSD.ClassName);

                var ship = PhotonNetwork.Instantiate(shipPrefabName, Vector3.zero, Quaternion.identity).GetComponent<Ship>();
                ship.SSDName = data.SSD.name;
                ship.Name = data.ShipName;

                var entry = Instantiate(shipsListEntryPrefab).GetComponent<ShipsListEntry>();
                entry.Ship = ship;
                entry.OnDelete.AddListener(() =>
                {
                    Destroy(entry.gameObject);
                    PhotonNetwork.Destroy(ship.gameObject);
                });
                entry.transform.SetParent(playerShipsListContent);
            }
            else
            {
                var color = totalCostText.color;
                totalCostText.color = Color.red;
                StartCoroutine(FlashTotalCost());
            }
        }

        private IEnumerator FlashTotalCost()
        {
            totalCostText.color = Color.red;
            yield return new WaitForSeconds(1);
            totalCostText.color = Color.white;
        }

        private int GetPlayerShipsTotalCost()
        {
            return PhotonNetwork.LocalPlayer.GetShips().Sum(ship => ship.Cost);
        }

        private void UpdateTotalCost()
        {
            totalCostText.text = string.Format("{0} / {1}",
                GetPlayerShipsTotalCost(),
                PhotonNetwork.CurrentRoom.GetMaxPoints());
        }
    }
}
