using UnityEngine;

namespace ST
{
    [CreateAssetMenu(menuName = "SSD/Ship SSD")]
    public class ShipSSD : ScriptableObject
    {
        public string ClassName;
        public ShipCategory Category;
        public Faction Faction;

        public string[] SampleNames;

        public int BaseCost;

        public string PickRandomName()
        {
            if (SampleNames.Length == 0)
            {
                return ClassName;
            }

            return SampleNames[Random.Range(0, SampleNames.Length - 1)];
        }
    }
}