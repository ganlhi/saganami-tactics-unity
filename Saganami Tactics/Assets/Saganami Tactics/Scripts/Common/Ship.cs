using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace ST
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    public class Ship : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Editor customization
        [SerializeField]
        private LineRenderer VectorLine;
        #endregion

        #region Constants
        private static readonly float MarkerMinDistance = .1f;
        #endregion

        #region Public variables
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

        public bool IsMoving { get; private set; }
        #endregion

        #region Unity callbacks
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SSD = Resources.Load<ShipSSD>("SSD/" + SSDName);
            var color = GameSettings.GetColor(photonView.Owner.GetColorIndex());
            VectorLine.startColor = color;
            VectorLine.endColor = color;
        }

        private void Update()
        {
            VectorLine.SetPosition(0, transform.position);
            VectorLine.SetPosition(1, EndMarker.transform.position);
        }
        #endregion

        #region Photon callbacks
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
        #endregion

        #region Public methods
        public void PlaceMarkers()
        {
            PlaceMarker(MiddleMarker, .5f);
            PlaceMarker(EndMarker);
        }
        #endregion

        #region Private methods
        private void PlaceMarker(ShipMarker marker, float velocityMult = 1)
        {
            marker.transform.position = transform.position + velocityMult * Velocity;
            marker.transform.rotation = transform.rotation;

            marker.gameObject.SetActive(transform.position.DistanceTo(marker.transform.position) >= MarkerMinDistance);

            // TODO Remove when plotting implemented
            marker.transform.rotation *= Quaternion.AngleAxis(velocityMult * 60, Vector3.right) * Quaternion.AngleAxis(velocityMult * 30, Vector3.forward);
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

        public void AutoMove()
        {
            ShipMarker marker;
            switch (PlayController.Instance.Step)
            {
                case TurnStep.HalfMove:
                    marker = MiddleMarker;
                    break;
                case TurnStep.FullMove:
                    marker = EndMarker;
                    break;
                default:
                    return;
            }

            StartCoroutine(MoveToMarker(marker));
        }

        private IEnumerator MoveToMarker(ShipMarker marker)
        {
            var startPos = transform.position;
            var toPos = marker.transform.position;

            var startRot = transform.rotation;
            var toRot = marker.transform.rotation;

            var duration = 2f;
            var elapsedTime = 0f;

            IsMoving = true;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, toPos, elapsedTime / duration);
                transform.rotation = Quaternion.Lerp(startRot, toRot, elapsedTime / duration);
                yield return null;
            }

            marker.gameObject.SetActive(false);
            IsMoving = false;
        }
        #endregion
    }
}
