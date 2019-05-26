using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameState : ScriptableObject
{
    public List<ShipState> Ships;
    public int Turn;
}

[Serializable]
public struct ShipState
{
    public string Name;
    public Vector3 Position;
    public Attitude Attitude;
    public Vector3 Velocity;
    public ShipStats Stats;
    public int OwnerColorIndex;
}
