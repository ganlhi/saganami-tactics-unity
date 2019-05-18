using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace ST
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    public class ShipMarker : MonoBehaviourPun, IPunObservable
    {
        #region Editor customization
#pragma warning disable 0649
        [SerializeField] private GameObject forLocalPlayer;
        [SerializeField] private GameObject forDistantPlayer;
        [SerializeField] private MeshRenderer forDistantPlayerMesh;
#pragma warning restore
        #endregion

        #region Public variables
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
        #endregion

        #region Unity callbacks
        private void Start()
        {
            forLocalPlayer.SetActive(photonView.IsMine);
            forDistantPlayer.SetActive(!photonView.IsMine);
            forDistantPlayerMesh.material.color = GameSettings.GetColor(photonView.Owner.GetColorIndex());
        }
        #endregion

        #region Photon callbacks
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
        #endregion

        #region Public methods
        public void SetActive(bool active)
        {
            photonView.RPC("RPC_SetActive", RpcTarget.All, active);
        }
        #endregion

        #region Private methods
        [PunRPC]
        private void RPC_SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        #endregion
    }
}
