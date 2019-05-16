using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
public class Ship : MonoBehaviour, IPointerClickHandler
{
    public Guid ID;
    public Side Side;
    public string Name;
    public ShipStats Stats;

    public Vector3 Position;
    public Vector3 Velocity;
    public Attitude Attitude;

    public Plotting Plotting = Plotting.Empty;

    public Marker.DisplayMode MarkersDisplayMode;

    public int AvailablePivots => Stats.MaxPivots - Plotting.Pivots.Count;
    public int AvailableRolls => Stats.MaxPivots - Plotting.Rolls.Count;

    private LineRenderer velocityLine;

    private Marker middleMarker;
    private Marker endMarker;

#pragma warning disable 0649
    [SerializeField] GameObject markerPrefab;
#pragma warning restore

    private void Start()
    {
        setInitialPositionAndAttitude();
        generateMarkers();
        createVelocityLine();
        updateVelocityLine();
    }

    private void Update()
    {
        updateVelocityLine();
        setMarkersDisplayMode();

        if (GameController.Instance.IsPlotting)
        {
            ApplyPlotting();
        }
    }

    private void createVelocityLine()
    {
        velocityLine = GetComponent<LineRenderer>();
        velocityLine.positionCount = 3;
    }

    private void updateVelocityLine()
    {
        velocityLine.SetPosition(0, Position);
        velocityLine.SetPosition(1, middleMarker.Visible ? middleMarker.Position : Position);
        velocityLine.SetPosition(2, endMarker.Visible ? endMarker.Position : Position);
    }

    private void setInitialPositionAndAttitude()
    {
        transform.position = Position;
        transform.rotation = Attitude.ToQuaternion();
    }

    private void generateMarkers()
    {
        if (middleMarker == null) { middleMarker = generateMarker(); }
        if (endMarker == null) { endMarker = generateMarker(); }
    }

    private Marker generateMarker()
    {
        var marker = GameObject.Instantiate(markerPrefab).GetComponent<Marker>();
        marker.Position = Position;
        marker.Attitude = Attitude;
        marker.Visible = false;
        return marker;
    }

    private void setMarkersDisplayMode()
    {
        if (middleMarker != null) { middleMarker.Mode = MarkersDisplayMode; }
        if (endMarker != null) { endMarker.Mode = MarkersDisplayMode; }
    }

    public void PlaceMarkers()
    {
        middleMarker.Position = Position + .5f * Velocity;
        middleMarker.Attitude = Attitude;
        middleMarker.Visible = true;

        endMarker.Position = Position + Velocity;
        endMarker.Attitude = Attitude;
        endMarker.Visible = true;
    }

    public void ApplyDisplacement()
    {
        var dispAmount = Plotting.Displacement;
        var thrustVector = middleMarker.transform.forward.normalized;

        middleMarker.Position += thrustVector * (dispAmount / 2f);
        endMarker.Position += thrustVector * dispAmount;
    }

    public void ApplyPlotting()
    {
        middleMarker.Attitude = Attitude + Plotting.HalfAttitudeChange;
        endMarker.Attitude = Attitude + Plotting.AttitudeChange;
    }

    public void ApplyThrust()
    {
        var thrustVector = middleMarker.transform.forward.normalized;
        var acc = thrustVector * Plotting.Thrust;

        Velocity += acc;

        Plotting = Plotting.Empty;
    }

    public void MoveToNextMarker()
    {
        if (middleMarker.Visible)
        {
            StartCoroutine(moveToMarker(middleMarker));
        }
        else if (endMarker.Visible)
        {
            StartCoroutine(moveToMarker(endMarker));
        }
    }

    private IEnumerator moveToMarker(Marker marker)
    {
        var startPos = transform.position;
        var toPos = marker.transform.position;

        var startRot = transform.rotation;
        var toRot = marker.transform.rotation;

        var duration = GameController.Instance.AutoMoveDuration;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, toPos, elapsedTime / duration);
            transform.rotation = Quaternion.Lerp(startRot, toRot, elapsedTime / duration);
            yield return null;
        }

        Position = marker.Position;
        Attitude = marker.Attitude;
        marker.Visible = false;
    }

    public void PlotThrust(int thr)
    {
        Plotting.Thrust = Mathf.Min(thr, Stats.MaxThrust);
    }

    public void PlotPivot(Pivot p)
    {
        if (Plotting.Pivots.Count < Stats.MaxPivots)
        {
            if (Plotting.IsDiagonal(p) && !Plotting.CanPivotOnDiagonals) return;

            Plotting.Pivots.Add(p);
        }
    }

    public void PlotRoll(Roll r)
    {
        if (Plotting.Rolls.Count < Stats.MaxRolls)
        {
            Plotting.Rolls.Add(r);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameController.Instance.SelectedShip = this;
    }
}

[Serializable]
public struct ShipStats
{
    public int MaxPivots;
    public int MaxRolls;
    public int MaxThrust;
}