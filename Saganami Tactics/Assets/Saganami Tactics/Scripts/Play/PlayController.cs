using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ST
{

    public class PlayController : MonoBehaviourPunCallbacks
    {
        public static PlayController Instance { get; private set; }

        public Ship SelectedShip { get; private set; }
        public int Turn { get; private set; }
        public TurnStep Step { get; private set; }

        public UnityEvent OnStepEnd = new UnityEvent();
        public UnityEvent OnStepStart = new UnityEvent();

        [SerializeField]
        private Moba_Camera cameraController;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FocusOnPlayerShip();

            // Start first turn
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_SetStep", RpcTarget.All, 1, TurnStep.Start);
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
        private void RPC_SetStep(int turn, TurnStep step)
        {
            var firstTurn = turn == 1 && step == TurnStep.Start;

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
            OnStepEnd.Invoke();
        }

        private void OnStartStep()
        {
            OnStepStart.Invoke();

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

        public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.Players.Values.All(p => p.IsReady()))
                {
                    NextStep();
                }
            }
        }

        private void NextStep()
        {
            var step = Step.Next();
            var turn = step == TurnStep.Start ? Turn + 1 : Turn;
            photonView.RPC("RPC_SetStep", RpcTarget.All, turn, step);
        }
    }
}