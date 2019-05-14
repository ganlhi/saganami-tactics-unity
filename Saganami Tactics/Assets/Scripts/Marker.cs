using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public enum DisplayMode { Dot, Ghost }

    public Vector3 Position;
    public Attitude Attitude;
    public bool Visible;
    public DisplayMode Mode;

    [SerializeField] GameObject dotModeDisplay;
    [SerializeField] GameObject ghostModeDisplay;

    private void setDisplayMode(DisplayMode mode)
    {
        dotModeDisplay.SetActive(mode == DisplayMode.Dot && Visible);
        ghostModeDisplay.SetActive(mode == DisplayMode.Ghost && Visible);
    }

    private void Update()
    {
        transform.position = Position;
        transform.rotation = Attitude.ToQuaternion();
        setDisplayMode(Mode);
    }
}
