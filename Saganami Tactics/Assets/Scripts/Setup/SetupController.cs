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
#pragma warning disable 0649
        [SerializeField] private SetupInfoPanel infoPanel;
        [SerializeField] private Transform playerShipsListContent;
        [SerializeField] private GameObject shipsListEntryPrefab;
        [SerializeField] private Button readyButton;
        [SerializeField] private Canvas dynamicCanvas;
        [SerializeField] private Canvas waitingCanvas;
        [SerializeField] private TMP_Text nbReadyText;
#pragma warning restore

        private void Start()
        {
            infoPanel.OnAddShip.AddListener(AddShip);
            readyButton.onClick.AddListener(SetReady);
        }

        private void SetReady()
        {
            PhotonNetwork.LocalPlayer.SetReady();
        }

        private void FixedUpdate()
        {
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

        private void AddShip(AddShipEventData data)
        {
            // Create the ship
            Debug.LogFormat("<color=green>Create ship</color> {0}", data.ShipName);

            var ship = PhotonNetwork.Instantiate("Prefabs/Ship", Vector3.zero, Quaternion.identity).GetComponent<Ship>();
            ship.Name = data.ShipName;
            PlaceShip(ship);

            var entry = Instantiate(shipsListEntryPrefab).GetComponent<ShipsListEntry>();
            entry.Ship = ship;
            entry.OnDelete.AddListener(() =>
            {
                Destroy(entry.gameObject);
                PhotonNetwork.Destroy(ship.gameObject);
            });
            entry.transform.SetParent(playerShipsListContent);

        }

        private void PlaceShip(Ship ship)
        {
            var deployPoint = GameSettings.GetDeploymentCenterPoint(PhotonNetwork.LocalPlayer.GetColorIndex());
            var offsetAxis = deployPoint.x != 0 ? Vector3.forward : Vector3.right;
            var offsetDir = 1;
            var offsetAmount = 0;
            var occupied = true;
            Vector3 point = deployPoint;

            while (occupied)
            {
                point = deployPoint + offsetAxis * offsetDir * offsetAmount;
                if (IsShipAtPoint(point))
                {
                    if (offsetDir == -1)
                    {
                        offsetDir = 1;
                        offsetAmount++;
                    }
                    else
                    {
                        offsetDir = -1;
                    }
                }
                else
                {
                    occupied = false;
                }
            }

            var rotation = Quaternion.LookRotation(deployPoint * -1, Vector3.up);
            ship.transform.position = point;
            ship.transform.rotation = rotation;

            ship.Velocity = ship.transform.forward * 4;
        }

        private bool IsShipAtPoint(Vector3 point)
        {
            return PhotonNetwork
                .FindGameObjectsWithComponent(typeof(Ship))
                .Any(ship => ship.transform.position == point);
        }
    }
}
