using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ST
{
    public class SetupController : MonoBehaviourPunCallbacks
    {
        static Vector3[] deployOffsets =
        {
            new Vector3(0,0,0),
            new Vector3(-1,0,0),
            new Vector3(1,0,0),
            new Vector3(-2,0,0),
            new Vector3(2,0,0),
            new Vector3(-3,0,0),
            new Vector3(3,0,0),
            new Vector3(-4,0,0),
            new Vector3(4,0,0),
            new Vector3(-5,0,0),
            new Vector3(5,0,0),

            new Vector3(0,0,-1),
            new Vector3(-1,0,-1),
            new Vector3(1,0,-1),
            new Vector3(-2,0,-1),
            new Vector3(2,0,-1),
            new Vector3(-3,0,-1),
            new Vector3(3,0,-1),
            new Vector3(-4,0,-1),
            new Vector3(4,0,-1),
            new Vector3(-5,0,-1),
            new Vector3(5,0,-1),

            new Vector3(0,0,1),
            new Vector3(-1,0,1),
            new Vector3(1,0,1),
            new Vector3(-2,0,1),
            new Vector3(2,0,1),
            new Vector3(-3,0,1),
            new Vector3(3,0,1),
            new Vector3(-4,0,1),
            new Vector3(4,0,1),
            new Vector3(-5,0,1),
            new Vector3(5,0,1),

            new Vector3(0,0,-2),
            new Vector3(-1,0,-2),
            new Vector3(1,0,-2),
            new Vector3(-2,0,-2),
            new Vector3(2,0,-2),
            new Vector3(-3,0,-2),
            new Vector3(3,0,-2),
            new Vector3(-4,0,-2),
            new Vector3(4,0,-2),
            new Vector3(-5,0,-2),
            new Vector3(5,0,-2),

            new Vector3(0,0,2),
            new Vector3(-1,0,2),
            new Vector3(1,0,2),
            new Vector3(-2,0,2),
            new Vector3(2,0,2),
            new Vector3(-3,0,2),
            new Vector3(3,0,2),
            new Vector3(-4,0,2),
            new Vector3(4,0,2),
            new Vector3(-5,0,2),
            new Vector3(5,0,2),

            new Vector3(0,0,-3),
            new Vector3(-1,0,-3),
            new Vector3(1,0,-3),
            new Vector3(-2,0,-3),
            new Vector3(2,0,-3),
            new Vector3(-3,0,-3),
            new Vector3(3,0,-3),
            new Vector3(-4,0,-3),
            new Vector3(4,0,-3),
            new Vector3(-5,0,-3),
            new Vector3(5,0,-3),

            new Vector3(0,0,3),
            new Vector3(-1,0,3),
            new Vector3(1,0,3),
            new Vector3(-2,0,3),
            new Vector3(2,0,3),
            new Vector3(-3,0,3),
            new Vector3(3,0,3),
            new Vector3(-4,0,3),
            new Vector3(4,0,3),
            new Vector3(-5,0,3),
            new Vector3(5,0,3),

            new Vector3(0,0,-4),
            new Vector3(-1,0,-4),
            new Vector3(1,0,-4),
            new Vector3(-2,0,-4),
            new Vector3(2,0,-4),
            new Vector3(-3,0,-4),
            new Vector3(3,0,-4),
            new Vector3(-4,0,-4),
            new Vector3(4,0,-4),
            new Vector3(-5,0,-4),
            new Vector3(5,0,-4),

            new Vector3(0,0,4),
            new Vector3(-1,0,4),
            new Vector3(1,0,4),
            new Vector3(-2,0,4),
            new Vector3(2,0,4),
            new Vector3(-3,0,4),
            new Vector3(3,0,4),
            new Vector3(-4,0,4),
            new Vector3(4,0,4),
            new Vector3(-5,0,4),
            new Vector3(5,0,4),

            new Vector3(0,0,-5),
            new Vector3(-1,0,-5),
            new Vector3(1,0,-5),
            new Vector3(-2,0,-5),
            new Vector3(2,0,-5),
            new Vector3(-3,0,-5),
            new Vector3(3,0,-5),
            new Vector3(-4,0,-5),
            new Vector3(4,0,-5),
            new Vector3(-5,0,-5),
            new Vector3(5,0,-5),

            new Vector3(0,0,5),
            new Vector3(-1,0,5),
            new Vector3(1,0,5),
            new Vector3(-2,0,5),
            new Vector3(2,0,5),
            new Vector3(-3,0,5),
            new Vector3(3,0,5),
            new Vector3(-4,0,5),
            new Vector3(4,0,5),
            new Vector3(-5,0,5),
            new Vector3(5,0,5),
        };

#pragma warning disable 0649
        [SerializeField] private Transform playerShipsListContent;
        [SerializeField] private GameObject shipsListEntryPrefab;
        [SerializeField] private Button readyButton;
        [SerializeField] private Canvas dynamicCanvas;
        [SerializeField] private Canvas waitingCanvas;
        [SerializeField] private TMP_Text nbReadyText;
        [SerializeField] private GameObject shipDeployPrefab;
        [SerializeField] private Transform redDeploymentZone;
        [SerializeField] private Transform blueDeploymentZone;
        [SerializeField] private Transform greenDeploymentZone;
        [SerializeField] private Transform yellowDeploymentZone;
#pragma warning restore

        private Dictionary<Vector3, ShipDeploy> positionnedShips = new Dictionary<Vector3, ShipDeploy>();

        private void Start()
        {
            readyButton.onClick.AddListener(SetReady);
        }

        private void SetReady()
        {
            PhotonNetwork.LocalPlayer.SetReady();
        }

        private void FixedUpdate()
        {
            readyButton.gameObject.SetActive(positionnedShips.Count > 0);
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
                PhotonNetwork.LoadLevel(GameSettings.SceneGame);
            }
        }

        public void AddShip(string shipName)
        {
            // Create the ship
            Debug.LogFormat("<color=green>Create ship</color> {0}", shipName);

            var ship = Instantiate(shipDeployPrefab, Vector3.zero, Quaternion.identity).GetComponent<ShipDeploy>();
            ship.State = new ShipState
            {
                Name = shipName,
                OwnerColorIndex = PhotonNetwork.LocalPlayer.GetColorIndex(),
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
                Velocity = Vector3.zero,
                Stats = new ShipStats(),
            };
            PlaceShip(ship);

            var entry = Instantiate(shipsListEntryPrefab).GetComponent<ShipsListEntry>();
            entry.Ship = ship;
            entry.OnDelete.AddListener(() =>
            {
                positionnedShips.Remove(ship.State.Position);
                Destroy(entry.gameObject);
                Destroy(ship.gameObject);
            });
            entry.transform.SetParent(playerShipsListContent);

        }

        private void PlaceShip(ShipDeploy ship)
        {
            Vector3 deployPoint;
            Quaternion offsetRotation;

            switch (PhotonNetwork.LocalPlayer.GetColorIndex())
            {
                case 0:
                    deployPoint = redDeploymentZone.position;
                    offsetRotation = redDeploymentZone.rotation;
                    break;
                case 1:
                    deployPoint = blueDeploymentZone.position;
                    offsetRotation = blueDeploymentZone.rotation;
                    break;
                case 2:
                    deployPoint = greenDeploymentZone.position;
                    offsetRotation = greenDeploymentZone.rotation;
                    break;
                case 3:
                    deployPoint = yellowDeploymentZone.position;
                    offsetRotation = yellowDeploymentZone.rotation;
                    break;
                default:
                    Debug.LogError("Bad color index");
                    Destroy(ship.gameObject);
                    return;
            }

            var occupied = true;
            Vector3 point = deployPoint;
            int altitude = 0;
            int offsetIndex = 0;

            while (occupied && altitude <= 5)
            {
                var offset = deployOffsets[offsetIndex];
                point = deployPoint + (offsetRotation * offset) + Vector3.up * altitude;

                if (!positionnedShips.ContainsKey(point))
                {
                    occupied = false;
                }
                else
                {
                    if (offsetIndex == deployOffsets.Length - 1)
                    {
                        offsetIndex = 0;
                        altitude += 1;
                    }
                    else
                    {
                        offsetIndex += 1;
                    }
                }
            }

            if (occupied)
            {
                Debug.LogError("No space available");
                Destroy(ship.gameObject);
                return;
            }

            var rotation = Quaternion.LookRotation(deployPoint * -1, Vector3.up);

            ship.State.Position = point;
            ship.State.Rotation = rotation;

            positionnedShips.Add(point, ship);
        }
    }
}
