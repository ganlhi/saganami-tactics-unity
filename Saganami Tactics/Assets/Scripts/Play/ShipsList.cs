using UnityEngine;

namespace ST
{
    public class ShipsList : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Transform content;
        [SerializeField] private GameObject itemPrefab;
#pragma warning restore

        private void Start()
        {
            FillList();
        }

        private void FillList()
        {
            var ships = PlayController.Instance.GetAllShips();
            foreach (var ship in ships)
            {
                var entry = Instantiate(itemPrefab).GetComponent<ShipButton>();
                entry.Ship = ship;
                entry.transform.SetParent(content);
            }
        }
    }
}
