using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameState : ScriptableObject
{
    public List<ShipState> Ships;
}

[Serializable]
public struct ShipState
{
    public Side Side;
    public string Name;
    public ShipStats Stats;
    public Vector3 Position;
    public Vector3 Velocity;
    public Attitude Attitude;
}