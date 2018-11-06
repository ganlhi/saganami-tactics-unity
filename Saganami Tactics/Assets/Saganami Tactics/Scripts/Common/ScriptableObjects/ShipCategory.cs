using UnityEngine;

namespace ST
{
    [CreateAssetMenu(menuName = "SSD/Ship Category")]
    public class ShipCategory : ScriptableObject
    {
        public string Name;
        public string Code;
    }
}