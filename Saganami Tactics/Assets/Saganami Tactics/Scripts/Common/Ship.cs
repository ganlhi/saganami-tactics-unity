using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #endregion Editor customization

        #region Public variables

        public ShipSSD SSD { get; private set; }

        private string ssdName;

        public string SSDName
        {
            get { return ssdName; }
            set
            {
                ssdName = value;
                SSD = Resources.Load<ShipSSD>("SSD/" + SSDName);
            }
        }

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

        public ReadOnlyCollection<Quaternion> PlottedPivots
        {
            get
            {
                return plottedPivots.AsReadOnly();
            }
        }

        public ReadOnlyCollection<Quaternion> PlottedRolls
        {
            get
            {
                return plottedRolls.AsReadOnly();
            }
        }

        public int MaxThrust
        {
            get
            {
                return SSD.InternalSystems.MaxThrust[0].y; // TODO take damages into account
            }
        }

        public int MaxPivots
        {
            get
            {
                return SSD.InternalSystems.Pivot[0]; // TODO take damages into account
            }
        }

        public int MaxRolls
        {
            get
            {
                return SSD.InternalSystems.Roll[0]; // TODO take damages into account
            }
        }

        public int UsedPivots
        {
            get
            {
                return plottedPivots.Count;
            }
        }

        public int UsedRolls
        {
            get
            {
                return plottedRolls.Count;
            }
        }

        public int MaxRange
        {
            get
            {
                return SSD.RangeBands[SSD.RangeBands.Length - 1].y; // TODO take damages into account;
            }
        }

        public Dictionary<Side, int> AvailableMissiles
        {
            get
            {
                return new Dictionary<Side, int>()
                {
                    { Side.Forward, Mathf.Min(SSD.ForwardSystems.MissileLaunchers, SSD.ForwardSystems.MissileAmmunitions) },
                    { Side.Aft, Mathf.Min(SSD.AftSystems.MissileLaunchers, SSD.AftSystems.MissileAmmunitions) },
                    { Side.Port, Mathf.Min(SSD.PortSystems.MissileLaunchers, SSD.PortSystems.MissileAmmunitions) },
                    { Side.Starboard, Mathf.Min(SSD.StarboardSystems.MissileLaunchers, SSD.StarboardSystems.MissileAmmunitions) },
                };
            }
        }

        #endregion Public variables

        #region Private variables

        private List<Quaternion> plottedPivots = new List<Quaternion>();
        private List<Quaternion> plottedRolls = new List<Quaternion>();

        #endregion Private variables

        #region Unity callbacks

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            var color = GameSettings.GetColor(photonView.Owner.GetColorIndex());
            VectorLine.startColor = color;
            VectorLine.endColor = color;
        }

        private void Update()
        {
            if (endMarkerViewId != -1)
            {
                VectorLine.enabled = true;
                VectorLine.SetPosition(0, transform.position);
                VectorLine.SetPosition(1, EndMarker.transform.position);
            }
            else
            {
                VectorLine.enabled = false;
            }
        }

        #endregion Unity callbacks

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
                stream.SendNext(Velocity);
                stream.SendNext(Thrust);
            }
            else
            {
                // Network ship, receive data
                SSDName = (string)stream.ReceiveNext();
                Name = (string)stream.ReceiveNext();
                middleMarkerViewId = (int)stream.ReceiveNext();
                endMarkerViewId = (int)stream.ReceiveNext();
                Velocity = (Vector3)stream.ReceiveNext();
                Thrust = (Vector3)stream.ReceiveNext();
            }
        }

        #endregion Photon callbacks

        #region Public methods

        public void ResetPivots(bool updateMarkers = false)
        {
            plottedPivots.Clear();
            if (updateMarkers)
            {
                PlaceMarkers();
            }
        }

        public void ResetRolls(bool updateMarkers = false)
        {
            plottedRolls.Clear();
            if (updateMarkers)
            {
                PlaceMarkers();
            }
        }

        public void ResetPlottings()
        {
            ResetPivots();
            ResetRolls();
            Thrust = Vector3.zero;
        }

        public void PlaceMarkers()
        {
            PlaceMarker(MiddleMarker, true);
            PlaceMarker(EndMarker, false);
        }

        public List<TargetData> IdentifyTargets(Salvo salvo, Side side)
        {
            var from = salvo == Salvo.Late ? MiddleMarker.transform : transform;

            var targets = new List<TargetData>();

            foreach (var ship in PlayController.Instance.GetAllShips())
            {
                if (ship.photonView.IsMine)
                {
                    continue;
                }

                // Get target transform
                var to = salvo == Salvo.Early
                    ? ship.transform
                    : (salvo == Salvo.Middle ? ship.MiddleMarker.transform : ship.EndMarker.transform);

                // Check distance
                var distance = Mathf.CeilToInt(from.position.DistanceTo(to.position));
                if (MaxRange < distance)
                {
                    continue;
                }

                // Check missiles
                var missiles = AvailableMissiles[side];
                if (missiles <= 0)
                {
                    continue;
                }

                var attackBearing = Bearing.Compute(from, to.position);
                if (attackBearing.Wedge.HasValue || attackBearing.Side != side)
                {
                    continue;
                }

                var defenseBearing = Bearing.Compute(to, from.position);

                var mql = GetMQLAtDistance(distance);

                targets.Add(new TargetData
                {
                    Attacker = this,
                    Target = ship,
                    Distance = distance,
                    Mql = mql.Value,
                    Missiles = missiles,
                    Salvo = salvo,
                    AttackingSide = side,
                    TargetSide = defenseBearing.Side,
                    TargetSideWall = defenseBearing.SideWall,
                    TargetWedge = defenseBearing.Wedge
                });
            }

            return targets;
        }

        public int? GetMQLAtDistance(int distance)
        {
            var bands = SSD.RangeBands.Where(rb => rb.y <= distance);
            if (bands.Count() == 0)
            {
                return null;
            }
            return bands.Last().z;
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

        public void PlotPivot(Quaternion rot)
        {
            plottedPivots.Add(rot);
            PlaceMarkers();
        }

        public void PlotRoll(Quaternion rot)
        {
            plottedRolls.Add(rot);
            PlaceMarkers();
        }

        public void PlotThrust(int amount)
        {
            amount = Mathf.Clamp(amount, 0, MaxThrust);
            Thrust = amount * MiddleMarker.transform.forward.normalized;
            PlaceMarkers();
        }

        public void ApplyThrust()
        {
            Velocity += Thrust;
        }

        #endregion Public methods

        #region Private methods

        private void PlaceMarker(ShipMarker marker, bool isHalf)
        {
            var velocityMult = isHalf ? .5f : 1f;
            marker.transform.position = transform.position + velocityMult * Velocity;

            var pivots = plottedPivots.Take(isHalf ? Mathf.FloorToInt(UsedPivots / 2) : UsedPivots);
            var rolls = plottedRolls.Take(isHalf ? Mathf.FloorToInt(UsedRolls / 2) : UsedRolls);

            var qPivot = pivots.Count() == 0
                ? Quaternion.identity
                : pivots.Aggregate(Quaternion.identity, (totalPivot, pivot) => totalPivot * pivot);

            var qRoll = rolls.Count() == 0
                ? Quaternion.identity
                : rolls.Aggregate(Quaternion.identity, (totalRoll, roll) => totalRoll * roll);

            marker.transform.rotation = transform.rotation * qPivot * qRoll;

            if (UsedPivots == 0)
            {
                marker.transform.position += Thrust * .5f * velocityMult;
            }

            marker.SetActive(transform.position != marker.transform.position);
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

            marker.SetActive(false);

            return marker;
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

            marker.SetActive(false);
            IsMoving = false;
        }

        #endregion Private methods
    }
}