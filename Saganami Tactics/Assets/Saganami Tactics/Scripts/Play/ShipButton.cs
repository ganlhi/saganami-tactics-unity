using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class ShipButton : MonoBehaviour
    {
        #region Public variables

        public Ship Ship;

        #endregion Public variables

        #region Editor customization

        [SerializeField]
        private Image colorCorner;

        [SerializeField]
        private Button focusButton;

        [SerializeField]
        private Button mainButton;

        [SerializeField]
        private TMP_Text shipName;

        #endregion Editor customization

        #region Unity callbacks

        private void Start()
        {
            colorCorner.color = GameSettings.GetColor(Ship.photonView.Owner.GetColorIndex());
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

        #endregion Unity callbacks
    }
}