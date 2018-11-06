using UnityEngine;

namespace ST
{
    [CreateAssetMenu(menuName = "SSD/Faction")]
    public class Faction : ScriptableObject
    {
        public string Name;
        public Sprite Flag;
    }
}