using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ST
{
    public class SsdListEntry : MonoBehaviour
    {
        public ShipSSD SSD;

        [SerializeField]
        private Image flag;

        [SerializeField]
        private TMP_Text className;

        [SerializeField]
        private TMP_Text category;

        [SerializeField]
        private TMP_Text cost;

        [SerializeField]
        private Button selectButton;

        public UnityEvent OnSelect { get { return selectButton.onClick; } }

        private void Start()
        {
            flag.sprite = SSD.Faction.Flag;
            className.text = SSD.ClassName;
            category.text = string.Format("{0} ({1})", SSD.Category.Name, SSD.Category.Code);
            cost.text = SSD.BaseCost.ToString();
        }
    }
}
