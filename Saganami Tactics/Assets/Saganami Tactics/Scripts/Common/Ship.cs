using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace ST
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    public class Ship : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField]
        private LineRenderer VectorLine;

        public ShipSSD SSD { get; private set; }
        public string SSDName;
        public string Name;
        public Vector3 Velocity = Vector3.zero;
        public Vector3 Thrust = Vector3.zero;

        private int middleMarkerViewId = -1;
        private ShipMarker middleMarker;
        public ShipMarker MiddleMarker
        {
            get
            {
                if (middleMarker != null)
                {
                    return middleMarker;
                }

                middleMarker = GetOrCreateMarker(middleMarkerViewId);
                middleMarkerViewId = middleMarker.photonView.ViewID;
                return middleMarker;
            }
        }

        private int endMarkerViewId = -1;
        private ShipMarker endMarker;
        public ShipMarker EndMarker
        {
            get
            {
                if (endMarker != null)
                {
                    return endMarker;
                }

                endMarker = GetOrCreateMarker(endMarkerViewId);
                endMarkerViewId = endMarker.photonView.ViewID;
                return endMarker;
            }
        }

        public int Cost
        {
            get { return SSD.BaseCost; }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SSD = Resources.Load<ShipSSD>("SSD/" + SSDName);
            VectorLine.endColor = GameSettings.GetColor(photonView.Owner.GetColorIndex());
        }

        private void Update()
        {
            VectorLine.SetPosition(0, transform.position);
            VectorLine.SetPosition(1, EndMarker.transform.position);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this ship: send the others our data
                stream.SendNext(SSDName);
                stream.SendNext(Name);
                stream.SendNext(middleMarkerViewId);
                stream.SendNext(endMarkerViewId);
            }
            else
            {
                // Network ship, receive data
                SSDName = (string)stream.ReceiveNext();
                Name = (string)stream.ReceiveNext();
                middleMarkerViewId = (int)stream.ReceiveNext();
                endMarkerViewId = (int)stream.ReceiveNext();
            }
        }

        public void PlaceMarkers()
        {
            PlaceMarker(MiddleMarker, Velocity * .5f);
            PlaceMarker(EndMarker, Velocity);
        }

        private void PlaceMarker(ShipMarker marker, Vector3 offset)
        {
            marker.transform.position = transform.position + offset;
            marker.transform.rotation = transform.rotation;

            marker.gameObject.SetActive((marker.transform.position - transform.position).magnitude >= 1);
        }

        private ShipMarker GetOrCreateMarker(int viewId)
        {
            ShipMarker marker = null;

            if (viewId != -1)
            {
                marker = PlayController.Instance.GetAllShipMarkers()
                    .FirstOrDefault(m => m.photonView.ViewID == viewId);
            }

            if (marker != null)
            {
                return marker;
            }

            marker = PhotonNetwork.Instantiate("Prefabs/ShipMarker", transform.position, transform.rotation)
                .GetComponent<ShipMarker>();
            marker.ShipViewId = photonView.ViewID;
            marker.gameObject.SetActive(false);

            return marker;
        }
    }
}
