using Photon.Pun;
using UnityEngine;

namespace ST
{
    public class PlayController : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        private Moba_Camera cameraController;

        private void Start()
        {
            FocusOnPlayerShip();
        }

        private void FocusOnPlayerShip()
        {
            var ship = PhotonNetwork.LocalPlayer.GetShips()[0];
            cameraController.SetTargetTransform(ship.transform);
            cameraController.settings.cameraLocked = true;
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}