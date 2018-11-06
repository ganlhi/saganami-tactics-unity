using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class ShipButton : MonoBehaviour
    {
        public Ship Ship;

        [SerializeField]
        private TMP_Text shipName;

        [SerializeField]
        private Image colorCorner;

        [SerializeField]
        private Button mainButton;

        [SerializeField]
        private Button focusButton;

        private void Start()
        {
            colorCorner.color = GameSettings.GetColor(Ship.PV.Owner.GetColorIndex());
            shipName.text = Ship.Name;

            focusButton.onClick.AddListener(() =>
            {
                PlayController.Instance.ToggleFocusOnShip(Ship);
            });

            mainButton.onClick.AddListener(() =>
            {
                PlayController.Instance.SelectShip(Ship);
            });
        }
    }
}
