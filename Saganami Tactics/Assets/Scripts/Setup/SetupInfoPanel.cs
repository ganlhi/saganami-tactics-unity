using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class SetupInfoPanel : MonoBehaviour
    {
        public AddShipEvent OnAddShip = new AddShipEvent();

#pragma warning disable 0649
        [SerializeField] private TMP_InputField shipNameField;
        [SerializeField] private Button addButton;
#pragma warning restore

        private void Start()
        {
            addButton.onClick.AddListener(AddShip);
        }

        private void AddShip()
        {
            OnAddShip.Invoke(new AddShipEventData
            {
                ShipName = shipNameField.text
            });
        }

        private void FixedUpdate()
        {
            addButton.interactable = !string.IsNullOrEmpty(shipNameField.text);
        }
    }
}
