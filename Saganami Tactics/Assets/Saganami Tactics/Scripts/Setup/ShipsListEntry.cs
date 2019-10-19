using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ST
{
    public class ShipsListEntry : MonoBehaviour
    {
        public Ship Ship;

        [SerializeField]
        private TMP_Text shipName;

        [SerializeField]
        private TMP_Text classAndCategory;

        [SerializeField]
        private TMP_Text cost;

        [SerializeField]
        private Button deleteButton;

        public UnityEvent OnDelete { get { return deleteButton.onClick; } }

        private void Start()
        {
            shipName.text = Ship.Name;
            classAndCategory.text = $"{Ship.SSD.ClassName} ({Ship.SSD.Category.Code})";
            cost.text = Ship.Cost.ToString();
        }
    }
}
