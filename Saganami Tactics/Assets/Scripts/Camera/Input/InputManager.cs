﻿using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
    public delegate void MoveInputHandler(Vector3 moveVector);
    public delegate void RotateInputHandler(Vector2 rotateAmount);
    public delegate void ZoomInputHandler(float zoomAmount);

}
