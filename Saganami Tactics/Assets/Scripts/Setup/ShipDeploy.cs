using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST
{
    public class ShipDeploy : MonoBehaviour
    {
        public ShipState State;

        LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            //var color = GameSettings.GetColor(State.OwnerColorIndex);
            //lineRenderer.startColor = lineRenderer.endColor = color;

            //foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            //{
            //    mesh.material.color = color;
            //}
        }

        private void Update()
        {
            transform.position = State.Position;
            transform.rotation = State.Rotation;

            lineRenderer.SetPosition(0, State.Position);
            lineRenderer.SetPosition(1, State.Position + State.Velocity);
        }
    }
}