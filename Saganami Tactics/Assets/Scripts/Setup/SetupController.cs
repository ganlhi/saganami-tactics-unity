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
        [SerializeField] private Transform controlPanel;
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
        [SerializeField] private SelectionMarker selectionMarker;
#pragma warning restore

        public ShipDeploy SelectedShip;
        private Dictionary<Vector3, ShipDeploy> positionnedShips = new Dictionary<Vector3, ShipDeploy>();

        #region Unity callbacks

        private void Start()
        {
            readyButton.onClick.AddListener(SetReady);
        }

        private void FixedUpdate()
        {
            readyButton.gameObject.SetActive(positionnedShips.Count > 0);
            controlPanel.gameObject.SetActive(SelectedShip != null);

            if (SelectedShip != null)
            {
                selectionMarker.SelectedObject = SelectedShip.gameObject;
                selectionMarker.SelectedObjectName = SelectedShip.State.Name;
            }
        }

        #endregion

        #region Photon callbacks

        public override void OnPlayerPropertiesUpdate(Player player, Hashtable changedProps)
        {
            var ready = PhotonNetwork.LocalPlayer.IsReady();
            dynamicCanvas.gameObject.SetActive(!ready);
            waitingCanvas.gameObject.SetActive(ready);

            UpdateNbReady();
        }

        #endregion

        #region Readiness

        private void SetReady()
        {
            PhotonNetwork.LocalPlayer.SetReady();
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

        #endregion

        #region Ships creation
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
            ship.OnSelect.AddListener((s) => { SelectedShip = s; });
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

        private Transform getShipDeploymentZone()
        {
            switch (PhotonNetwork.LocalPlayer.GetColorIndex())
            {
                case 0:
                    return redDeploymentZone;
                case 1:
                    return blueDeploymentZone;
                case 2:
                    return greenDeploymentZone;
                case 3:
                    return yellowDeploymentZone;
                default:
                    throw new System.Exception("Bad color index");
            }
        }

        private void PlaceShip(ShipDeploy ship)
        {
            var dz = getShipDeploymentZone();

            Vector3 deployPoint = dz.position;
            Quaternion offsetRotation = dz.rotation;

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

        #endregion

        #region Ships movement

        private Vector3 getPointForward()
        {
            if (SelectedShip == null) return Vector3.zero;

            var dz = getShipDeploymentZone();
            return dz.forward + SelectedShip.State.Position;
        }
        private Vector3 getPointBackward()
        {
            if (SelectedShip == null) return Vector3.zero;

            var dz = getShipDeploymentZone();
            return -dz.forward + SelectedShip.State.Position;
        }
        private Vector3 getPointLeft()
        {
            if (SelectedShip == null) return Vector3.zero;

            var dz = getShipDeploymentZone();
            return -dz.right + SelectedShip.State.Position;
        }
        private Vector3 getPointRight()
        {
            if (SelectedShip == null) return Vector3.zero;

            var dz = getShipDeploymentZone();
            return dz.right + SelectedShip.State.Position;
        }
        private Vector3 getPointUp()
        {
            if (SelectedShip == null) return Vector3.zero;

            return Vector3.up + SelectedShip.State.Position;
        }
        private Vector3 getPointDown()
        {
            if (SelectedShip == null) return Vector3.zero;

            return Vector3.down + SelectedShip.State.Position;
        }

        public bool CanGoForward()
        {
            if (SelectedShip == null) return false;
            var point = getPointForward();
            var dz = getShipDeploymentZone();
            return !positionnedShips.ContainsKey(point) && Vector3.Project(point - dz.position, dz.forward).magnitude <= 5;
        }
        public bool CanGoBackward()
        {
            if (SelectedShip == null) return false;
            var point = getPointBackward();
            var dz = getShipDeploymentZone();
            return !positionnedShips.ContainsKey(point) && Vector3.Project(point - dz.position, -dz.forward).magnitude <= 5;
        }
        public bool CanGoLeft()
        {
            if (SelectedShip == null) return false;
            var point = getPointLeft();
            var dz = getShipDeploymentZone();
            return !positionnedShips.ContainsKey(point) && Vector3.Project(point - dz.position, -dz.right).magnitude <= 5;
        }
        public bool CanGoRight()
        {
            if (SelectedShip == null) return false;
            var point = getPointRight();
            var dz = getShipDeploymentZone();
            return !positionnedShips.ContainsKey(point) && Vector3.Project(point - dz.position, dz.right).magnitude <= 5;
        }
        public bool CanGoUp()
        {
            if (SelectedShip == null) return false;
            var point = getPointUp();
            return !positionnedShips.ContainsKey(point) && point.y <= 5;
        }
        public bool CanGoDown()
        {
            if (SelectedShip == null) return false;
            var point = getPointDown();
            return !positionnedShips.ContainsKey(point) && point.y >= 0;
        }

        public void MoveForward()
        {
            if (CanGoForward())
            {
                positionnedShips.Remove(SelectedShip.State.Position);
                SelectedShip.State.Position = getPointForward();
                positionnedShips.Add(SelectedShip.State.Position, SelectedShip);
            }
        }

        public void MoveBackward()
        {
            if (CanGoBackward())
            {
                positionnedShips.Remove(SelectedShip.State.Position);
                SelectedShip.State.Position = getPointBackward();
                positionnedShips.Add(SelectedShip.State.Position, SelectedShip);
            }
        }

        public void MoveLeft()
        {
            if (CanGoLeft())
            {
                positionnedShips.Remove(SelectedShip.State.Position);
                SelectedShip.State.Position = getPointLeft();
                positionnedShips.Add(SelectedShip.State.Position, SelectedShip);
            }
        }

        public void MoveRight()
        {
            if (CanGoRight())
            {
                positionnedShips.Remove(SelectedShip.State.Position);
                SelectedShip.State.Position = getPointRight();
                positionnedShips.Add(SelectedShip.State.Position, SelectedShip);
            }
        }

        public void MoveUp()
        {
            if (CanGoUp())
            {
                positionnedShips.Remove(SelectedShip.State.Position);
                SelectedShip.State.Position = getPointUp();
                positionnedShips.Add(SelectedShip.State.Position, SelectedShip);
            }
        }

        public void MoveDown()
        {
            if (CanGoDown())
            {
                positionnedShips.Remove(SelectedShip.State.Position);
                SelectedShip.State.Position = getPointDown();
                positionnedShips.Add(SelectedShip.State.Position, SelectedShip);
            }
        }

        #endregion
    }
}
