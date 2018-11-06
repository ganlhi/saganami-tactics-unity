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

        [SerializeField]
        private Moba_Camera cameraController;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FocusOnPlayerShip();
        }

        private void FocusOnPlayerShip()
        {
            var ship = PhotonNetwork.LocalPlayer.GetShips()[0];
            ToggleFocusOnShip(ship);
            SelectShip(ship);
        }

        internal void ToggleFocusOnShip(Ship ship)
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

        internal void SelectShip(Ship ship)
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
    }
}