using UnityEngine;

public class KeyboardInputManager : InputManager
{
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            OnMoveInput?.Invoke(Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            OnMoveInput?.Invoke(Vector3.back);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            OnMoveInput?.Invoke(Vector3.left);
        }
        if (Input.GetKey(KeyCode.D))
        {
            OnMoveInput?.Invoke(Vector3.right);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            OnMoveInput?.Invoke(Vector3.up);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            OnMoveInput?.Invoke(Vector3.down);
        }

        if (Input.GetKey(KeyCode.A))
        {
            OnRotateInput?.Invoke(Vector2.left);
        }
        if (Input.GetKey(KeyCode.E))
        {
            OnRotateInput?.Invoke(Vector2.right);
        }
        if (Input.GetKey(KeyCode.R))
        {
            OnRotateInput?.Invoke(Vector2.up);
        }
        if (Input.GetKey(KeyCode.F))
        {
            OnRotateInput?.Invoke(Vector2.down);
        }

        if (Input.GetKey(KeyCode.X))
        {
            OnZoomInput?.Invoke(1f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            OnZoomInput?.Invoke(-1f);
        }
    }
}
