using UnityEngine;

public class MouseInputManager : InputManager
{
    public float deadBand = 10f;
    public float zoomMultiplier = 3f;
    public float rotateMultiplier = 3f;

    Vector2Int screen;
    Vector2? previousMousePosition;

#pragma warning disable 0067
    public static event MoveInputHandler OnMoveInput;
#pragma warning restore
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;

    private void Awake()
    {
        screen = new Vector2Int(Screen.width, Screen.height);
    }
    private void Update()
    {
        Vector3 mp = Input.mousePosition;
        bool mouseValid = mp.y <= screen.y && mp.y >= 0 && mp.x <= screen.x && mp.x >= 0;
        if (!mouseValid) return;

        if (Input.GetMouseButtonUp(1))
        {
            previousMousePosition = null;
        }
        else if (Input.GetMouseButton(1))
        {
            if (previousMousePosition.HasValue)
            {
                var rot = Vector2.zero;

                if (previousMousePosition?.x - mp.x > deadBand)
                {
                    rot += Vector2.left * rotateMultiplier;
                }
                else if (mp.x - previousMousePosition?.x > deadBand)
                {
                    rot += Vector2.right * rotateMultiplier;
                }

                if (previousMousePosition?.y - mp.y > deadBand)
                {
                    rot += Vector2.up * rotateMultiplier;
                }
                else if (mp.y - previousMousePosition?.y > deadBand)
                {
                    rot += Vector2.down * rotateMultiplier;
                }

                OnRotateInput?.Invoke(rot);
            }

            previousMousePosition = new Vector2(mp.x, mp.y);
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            OnZoomInput?.Invoke(-zoomMultiplier);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            OnZoomInput?.Invoke(zoomMultiplier);
        }
    }
}
