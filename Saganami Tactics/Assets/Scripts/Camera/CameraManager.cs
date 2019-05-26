using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera positioning")]
    public Vector2 cameraOffset = new Vector2(10f, 14f);

    [Header("Move controls")]
    public float inOutSpeed = 5f;
    public float lateralSpeed = 5f;
    public float verticalSpeed = 5f;
    public float rotateSpeed = 45f;

    [Header("Zoom controls")]
    public float zoomSpeed = 4f;
    public float nearZoomLimit = 2f;
    public float farZoomLimit = 16f;
    public float startingZoom = 5f;

    IZoomStrategy zoomStrategy;
    Vector3 frameMove;
    Vector2 frameRotate;
    float frameZoom;
    Camera cam;
    Transform arm;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        arm = cam.transform.parent;

        cam.transform.localPosition = new Vector3(0, cameraOffset.y, -cameraOffset.x);
        cam.transform.LookAt(transform.position);

        zoomStrategy = new PerspectiveZoomStrategy(cam, cameraOffset, startingZoom);
    }

    private void OnEnable()
    {
        KeyboardInputManager.OnMoveInput += UpdateFrameMove;
        KeyboardInputManager.OnRotateInput += UpdateFrameRotate;
        KeyboardInputManager.OnZoomInput += UpdateFrameZoom;
        MouseInputManager.OnMoveInput += UpdateFrameMove;
        MouseInputManager.OnRotateInput += UpdateFrameRotate;
        MouseInputManager.OnZoomInput += UpdateFrameZoom;
    }

    private void OnDisable()
    {
        KeyboardInputManager.OnMoveInput -= UpdateFrameMove;
        KeyboardInputManager.OnRotateInput -= UpdateFrameRotate;
        KeyboardInputManager.OnZoomInput -= UpdateFrameZoom;
        MouseInputManager.OnMoveInput -= UpdateFrameMove;
        MouseInputManager.OnRotateInput -= UpdateFrameRotate;
        MouseInputManager.OnZoomInput -= UpdateFrameZoom;
    }

    private void LateUpdate()
    {
        var zoomedMoveCoeff = Mathf.Lerp(.5f, 4f, zoomStrategy.CurrentZoomLevel / (farZoomLimit - nearZoomLimit));

        if (frameMove != Vector3.zero)
        {
            Vector3 speedModFrameMove = new Vector3(
                frameMove.x * lateralSpeed * zoomedMoveCoeff,
                frameMove.y * verticalSpeed * zoomedMoveCoeff,
                frameMove.z * inOutSpeed * zoomedMoveCoeff);
            transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
        }

        if (frameRotate != Vector2.zero)
        {
            transform.Rotate(Vector3.up, frameRotate.x * Time.deltaTime * rotateSpeed);
            arm.transform.Rotate(Vector3.right, frameRotate.y * Time.deltaTime * rotateSpeed, Space.Self);
        }

        if (frameZoom < 0f)
        {
            zoomStrategy.ZoomIn(cam, Time.deltaTime * Mathf.Abs(frameZoom) * zoomSpeed, nearZoomLimit);
        }
        else if (frameZoom > 0f)
        {
            zoomStrategy.ZoomOut(cam, Time.deltaTime * frameZoom * zoomSpeed, farZoomLimit);
        }

        frameMove = Vector3.zero;
        frameRotate = Vector2.zero;
        frameZoom = 0f;
    }

    private void UpdateFrameMove(Vector3 moveVector)
    {
        frameMove += moveVector;
    }

    private void UpdateFrameRotate(Vector2 rotateAmount)
    {
        frameRotate += rotateAmount;
    }

    private void UpdateFrameZoom(float zoomAmount)
    {
        frameZoom += zoomAmount;
    }
}
