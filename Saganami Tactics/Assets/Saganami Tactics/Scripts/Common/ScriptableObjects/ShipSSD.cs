using System;
using System.Collections.Generic;
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

        public List<RangeBand> RangeBands = new List<RangeBand>();
        public List<InternalSystem> InternalSystems = new List<InternalSystem>();

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
    public struct RangeBand
    {
        public Vector2Int Range;
        public int MQL;
    }

    public enum SystemType
    {
        Bridge,
        FlagBridge,
        LifeSupport,
        Communications,
        ECM,
        Pivot,
        Roll,
        FwdImpeller,
        AftImpeller,
        HyperGenerator,
        Hull,
        StructuralIntegrity,
    }

    [Serializable]
    public struct InternalSystem
    {
        public SystemType Type;
        public List<DamageableSlot> Slots;
    }

    public enum DamageableSlotType
    {
        Default,
        DestructionRisk,
        CoreCascade,
        UnsafeThrust,
        Warshawski,
        Destruction,
        Unbreakable,
    }

    [Serializable]
    public struct DamageableSlot
    {
        public DamageableSlotType Type;
        public bool HasValue;
        public int Value;
    }
}