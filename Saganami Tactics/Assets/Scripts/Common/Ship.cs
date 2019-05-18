﻿using Photon.Pun;
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

#pragma warning disable 0649
        [SerializeField] private LineRenderer VectorLine;
#pragma warning restore

        #endregion Editor customization

        #region Public variables

        //TODO replace with ShipStats
        //public ShipSSD SSD { get; private set; }


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
                return 3;
                //TODO replace with ShipStats
                //return SSD.InternalSystems.MaxThrust[0].y; // TODO take damages into account
            }
        }

        public int MaxPivots
        {
            get
            {
                return 6;
                //TODO replace with ShipStats
                //return SSD.InternalSystems.Pivot[0]; // TODO take damages into account
            }
        }

        public int MaxRolls
        {
            get
            {
                return 6;
                //TODO replace with ShipStats
                //return SSD.InternalSystems.Roll[0]; // TODO take damages into account
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
                stream.SendNext(Name);
                stream.SendNext(middleMarkerViewId);
                stream.SendNext(endMarkerViewId);
                stream.SendNext(Velocity);
                stream.SendNext(Thrust);
            }
            else
            {
                // Network ship, receive data
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