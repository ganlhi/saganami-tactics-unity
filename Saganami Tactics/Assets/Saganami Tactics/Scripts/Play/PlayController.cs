using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ST
{
    public class PlayController : MonoBehaviourPunCallbacks
    {
        #region Editor customization

        [SerializeField]
        private Moba_Camera cameraController;

        #endregion Editor customization

        #region Public variables

        public UnityEvent OnStepEnd = new UnityEvent();
        public UnityEvent OnStepStart = new UnityEvent();
        public static PlayController Instance { get; private set; }

        public Ship SelectedShip { get; private set; }
        public TurnStep Step { get; private set; }
        public int Turn { get; private set; }

        #endregion Public variables

        #region Public methods

        public List<ShipMarker> GetAllShipMarkers()
        {
            return PhotonNetwork
                .FindGameObjectsWithComponent(typeof(ShipMarker))
                .Select(go => go.GetComponent<ShipMarker>())
                .ToList();
        }

        public List<Ship> GetAllShips()
        {
            return PhotonNetwork
                .FindGameObjectsWithComponent(typeof(Ship))
                .Select(go => go.GetComponent<Ship>())
                .ToList();
        }

        public void SelectShip(Ship ship)
        {
            SelectedShip = ship;
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

        #endregion Public methods

        #region Private methods

        private void FocusOnPlayerShip()
        {
            var ship = PhotonNetwork.LocalPlayer.GetShips()[0];
            ToggleFocusOnShip(ship);
            SelectShip(ship);
        }

        private void MakeShipsMove()
        {
            var ships = PhotonNetwork.LocalPlayer.GetShips();
            if (ships.Count > 0)
            {
                StartCoroutine(WaitForShipsToFinishMoving(ships));
            }
        }

        private void NextStep()
        {
            PhotonNetwork.CurrentRoom.ResetPlayersReadiness();
            var step = Step.Next();
            var turn = step == TurnStep.Start ? Turn + 1 : Turn;
            photonView.RPC("RPC_SetStep", RpcTarget.All, turn, step);
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

                case TurnStep.HalfMove:
                case TurnStep.FullMove:
                    MakeShipsMove();
                    break;

                default:
                    break;
            }
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

        private IEnumerator WaitForShipsToFinishMoving(List<Ship> ships)
        {
            ships.ForEach(s => s.AutoMove());

            do
            {
                yield return null;
            } while (ships.Any(s => s.IsMoving));

            PhotonNetwork.LocalPlayer.SetReady();
        }

        #endregion Private methods

        #region Unity callbacks

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

        #endregion Unity callbacks

        #region Photon callbacks

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

        #endregion Photon callbacks
    }
}