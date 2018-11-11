using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ST
{
    [CreateAssetMenu(menuName = "SSD/Ship SSD")]
    public class ShipSSD : ScriptableObject
    {
        #region SSD properties

        public string ClassName;
        public ShipCategory Category;
        public Faction Faction;
        public int BaseCost;

        public InternalSystems InternalSystems;

        [Tooltip("For each entry, X and Y are start/end of band, Z is MQL")]
        public Vector3Int[] RangeBands;

        public SideSystems ForwardSystems;
        public SideSystems AftSystems;
        public SideSystems PortSystems;
        public SideSystems StarboardSystems;

        #endregion SSD properties

        #region Background info

        public string[] SampleNames;

        #endregion Background info

        #region Public methods

        public string PickRandomName()
        {
            if (SampleNames.Length == 0)
            {
                return ClassName;
            }

            return SampleNames[Random.Range(0, SampleNames.Length - 1)];
        }

        #endregion Public methods
    }

    [Serializable]
    public struct InternalSystems
    {
        public int Bridge;
        public int[] FlagBridge;
        public int LifeSupport;
        public int Communications;
        public int[] Ecm;
        public int[] Pivot;
        public int[] Roll;
        public int FwdImpeller;
        public int AftImpeller;
        public int Scale;
        public int CoreArmor;

        [Tooltip("For each entry, X is number of slots, Y is slots value")]
        public Vector2Int[] MaxThrust;

        public int HyperGenerator;
        public int HullPoints;
        public int[] DmgCtrlTeams;
        public StructuralSlot[] StructuralIntegrity;
    }

    [Serializable]
    public struct StructuralSlot
    {
        public StructuralSlotType Type;
        public int Value;
    }

    public enum StructuralSlotType
    {
        Default,
        DestructionRisk,
        CoreCascade,
        Destruction,
    }

    [Serializable]
    public struct RangeBand
    {
        public Vector2Int Range;
        public int MQL;
    }

    [Serializable]
    public struct SideSystems
    {
        public int[] FireControl;
        public int MissileAmmunitions;
        public int MissileLaunchers;
        public int MissileStrength;
        public int Lasers;
        public int[] LaserStrength;
        public int Grasers;
        public int[] GraserStrength;
        public ActiveDefenseSlot[] CounterMissiles;
        public ActiveDefenseSlot[] PointDefenses;
        public int Decoys;
        public int DecoysStrength;
    }

    public enum ActiveDefenseSlotType
    {
        Integer,
        TwoThird,
        OneThird,
    }

    [Serializable]
    public struct ActiveDefenseSlot
    {
        public ActiveDefenseSlotType Type;
        public int Value;
    }
}