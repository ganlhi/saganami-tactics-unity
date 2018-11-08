using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace ST
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    public class ShipMarker : MonoBehaviourPun, IPunObservable
    {
        [SerializeField]
        private GameObject forLocalPlayer;

        [SerializeField]
        private GameObject forDistantPlayer;

        public int ShipViewId = -1;

        public Ship Ship
        {
            get
            {
                if (ShipViewId == -1)
                {
                    return null;
                }

                return PlayController.Instance.GetAllShips()
                    .First(s => s.photonView.ViewID == ShipViewId);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this marker: send the others our data
                stream.SendNext(ShipViewId);
            }
            else
            {
                // Network marker, receive data
                ShipViewId = (int)stream.ReceiveNext();
            }
        }

        private void Start()
        {
            forLocalPlayer.SetActive(photonView.IsMine);
            forDistantPlayer.SetActive(!photonView.IsMine);
        }
    }
}
