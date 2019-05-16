using UnityEngine;

public class PerspectiveZoomStrategy : IZoomStrategy
{
    Vector3 normalizedCameraPosition;

    public float CurrentZoomLevel { get; private set; }

    public PerspectiveZoomStrategy(Camera cam, Vector3 offset, float startingZoom)
    {
        normalizedCameraPosition = new Vector3(0, Mathf.Abs(offset.y), -Mathf.Abs(offset.x)).normalized;
        CurrentZoomLevel = startingZoom;
        PositionCamera(cam);
    }

    private void PositionCamera(Camera cam)
    {
        cam.transform.localPosition = normalizedCameraPosition * CurrentZoomLevel;
    }

    public void ZoomIn(Camera cam, float delta, float nearZoomLimit)
    {
        if (CurrentZoomLevel <= nearZoomLimit) return;
        CurrentZoomLevel = Mathf.Max(CurrentZoomLevel - delta, nearZoomLimit);
        PositionCamera(cam);
    }

    public void ZoomOut(Camera cam, float delta, float farZoomLimit)
    {
        if (CurrentZoomLevel >= farZoomLimit) return;
        CurrentZoomLevel = Mathf.Min(CurrentZoomLevel + delta, farZoomLimit);
        PositionCamera(cam);
    }
}
