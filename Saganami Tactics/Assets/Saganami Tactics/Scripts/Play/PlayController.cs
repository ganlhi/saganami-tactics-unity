using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ST
{

    public class PlayController : MonoBehaviourPunCallbacks
    {
        public static PlayController Instance { get; private set; }

        public Ship SelectedShip { get; private set; }
        public int Turn { get; private set; }
        public TurnStep Step { get; private set; }

        [SerializeField]
        private Moba_Camera cameraController;

        private PhotonView PV;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            PV = GetComponent<PhotonView>();

            FocusOnPlayerShip();

            // Start first turn
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("SetStep", RpcTarget.All, 1, TurnStep.Start, true);
            }
        }

        public void ToggleFocusOnShip(Ship ship)
        {
            if (cameraController.settings.lockTargetTransform != ship.transform)
            {
                cameraController.SetTargetTransform(ship.transform);
                cameraController.settings.cameraLocked = true;
            }
            else
            {
                cameraController.SetTargetTransform(null);
                cameraController.settings.cameraLocked = false;
            }
        }

        public void SelectShip(Ship ship)
        {
            SelectedShip = ship;
        }

        public List<Ship> GetAllShips()
        {
            return PhotonNetwork
                .FindGameObjectsWithComponent(typeof(Ship))
                .Select(go => go.GetComponent<Ship>())
                .ToList();
        }

        public List<ShipMarker> GetAllShipMarkers()
        {
            return PhotonNetwork
                .FindGameObjectsWithComponent(typeof(ShipMarker))
                .Select(go => go.GetComponent<ShipMarker>())
                .ToList();
        }

        [PunRPC]
        private void SetStep(int turn, TurnStep step, bool firstTurn = false)
        {
            if (!firstTurn)
            {
                OnEndStep();
            }
            Turn = turn;
            Step = step;
            OnStartStep();
        }

        private void OnEndStep()
        {

        }

        private void OnStartStep()
        {
            switch (Step)
            {
                case TurnStep.Start:
                    PhotonNetwork.LocalPlayer.GetShips().ForEach(s => s.PlaceMarkers());
                    break;
                default:
                    break;
            }
        }

        private void FocusOnPlayerShip()
        {
            var ship = PhotonNetwork.LocalPlayer.GetShips()[0];
            ToggleFocusOnShip(ship);
            SelectShip(ship);
        }
    }
}