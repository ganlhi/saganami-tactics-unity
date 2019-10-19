using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class SetupInfoPanel : MonoBehaviour
    {
        public AddShipEvent OnAddShip = new AddShipEvent();

        private ShipSSD ssd;
        public ShipSSD SSD
        {
            get { return ssd; }
            set
            {
                ssd = value;
                UpdateUi();
            }
        }

        [SerializeField]
        private Transform[] rootElements;

        [SerializeField]
        private Image flag;

        [SerializeField]
        private TMP_Text factionName;

        [SerializeField]
        private TMP_Text className;

        [SerializeField]
        private TMP_Text category;

        [SerializeField]
        private TMP_Text cost;

        [SerializeField]
        private TMP_InputField shipNameField;

        [SerializeField]
        private Button randomNameButton;

        [SerializeField]
        private Button addButton;

        private void Start()
        {
            SetElementsVisible(SSD != null);

            randomNameButton.onClick.AddListener(PickRandomName);
            addButton.onClick.AddListener(AddShip);
        }

        private void AddShip()
        {
            OnAddShip.Invoke(new AddShipEventData
            {
                SSD = SSD,
                ShipName = shipNameField.text
            });
        }

        private void PickRandomName()
        {
            if (SSD)
            {
                shipNameField.text = SSD.PickRandomName();
            }
        }

        private void SetElementsVisible(bool visible)
        {
            foreach (var el in rootElements)
            {
                el.gameObject.SetActive(visible);
            }
        }

        private void FixedUpdate()
        {
            addButton.interactable = SSD != null && !string.IsNullOrEmpty(shipNameField.text);
        }

        private void UpdateUi()
        {
            SetElementsVisible(SSD != null);

            Debug.LogFormat("SSD Name: {0}", SSD.name);

            if (SSD == null)
            {
                return;
            }

            flag.sprite = SSD.Faction.Flag;
            factionName.text = SSD.Faction.Name;
            className.text = SSD.ClassName;
            category.text = $"{SSD.Category.Name} ({SSD.Category.Code})";
            cost.text = SSD.BaseCost.ToString();

            shipNameField.text = null;
        }
    }
}
