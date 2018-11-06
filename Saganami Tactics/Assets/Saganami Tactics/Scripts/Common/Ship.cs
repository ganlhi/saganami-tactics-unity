using Photon.Pun;
using UnityEngine;

namespace ST
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    public class Ship : MonoBehaviourPunCallbacks, IPunObservable
    {
        public PhotonView PV { get; private set; }
        public ShipSSD SSD { get; private set; }
        public string SSDName;
        public string Name;

        public int Cost
        {
            get { return SSD.BaseCost; }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            PV = GetComponent<PhotonView>();
            SSD = Resources.Load<ShipSSD>("SSD/" + SSDName);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this ship: send the others our data
                stream.SendNext(SSDName);
                stream.SendNext(Name);
            }
            else
            {
                // Network ship, receive data
                SSDName = (string)stream.ReceiveNext();
                Name = (string)stream.ReceiveNext();
            }
        }
    }
}
