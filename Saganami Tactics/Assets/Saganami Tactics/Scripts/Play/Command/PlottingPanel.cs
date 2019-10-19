using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class PlottingPanel : MonoBehaviour
    {
        #region Editor customization

        [SerializeField]
        private TMP_Text pivotUsedText;

        [SerializeField]
        private TMP_Text rollUsedText;

        [SerializeField]
        private Button pivotResetButton;

        [SerializeField]
        private Button rollResetButton;

        [SerializeField]
        private Slider thrustSlider;

        [SerializeField]
        private TMP_Text thrustText;

        #endregion Editor customization

        #region Public methods

        public void PitchUp()
        {
            PlayController.Instance.SelectedShip.PlotPivot(
                Quaternion.AngleAxis(-30, Vector3.right));
            UpdateUi();
        }

        public void PitchDown()
        {
            PlayController.Instance.SelectedShip.PlotPivot(
                Quaternion.AngleAxis(30, Vector3.right));
            UpdateUi();
        }

        public void YawLeft()
        {
            PlayController.Instance.SelectedShip.PlotPivot(
                Quaternion.AngleAxis(-30, Vector3.up));
            UpdateUi();
        }

        public void YawRight()
        {
            PlayController.Instance.SelectedShip.PlotPivot(
                Quaternion.AngleAxis(30, Vector3.up));
            UpdateUi();
        }

        public void RollLeft()
        {
            PlayController.Instance.SelectedShip.PlotRoll(
                Quaternion.AngleAxis(30, Vector3.forward));
            UpdateUi();
        }

        public void RollRight()
        {
            PlayController.Instance.SelectedShip.PlotRoll(
                Quaternion.AngleAxis(-30, Vector3.forward));
            UpdateUi();
        }

        public void Thrust()
        {
            PlayController.Instance.SelectedShip.PlotThrust((int)thrustSlider.value);
            UpdateUi();
        }

        public void ResetPivots()
        {
            PlayController.Instance.SelectedShip.ResetPivots(true);
            UpdateUi();
        }

        public void ResetRolls()
        {
            PlayController.Instance.SelectedShip.ResetRolls(true);
            UpdateUi();
        }

        #endregion Public methods

        #region Private methods

        private void UpdateUi()
        {
            var ship = PlayController.Instance.SelectedShip;
            if (ship == null)
            {
                Debug.LogError("This panel should not be displayed without a ship selected");
                return;
            }

            pivotUsedText.text = $"{ship.UsedPivots} / {ship.MaxPivots}";
            rollUsedText.text = $"{ship.UsedRolls} / {ship.MaxRolls}";

            pivotResetButton.interactable = ship.UsedPivots > 0;
            rollResetButton.interactable = ship.UsedRolls > 0;

            thrustSlider.maxValue = ship.MaxThrust;
            thrustSlider.value = ship.Thrust.magnitude;
            thrustText.text = thrustSlider.value.ToString();
        }

        #endregion Private methods

        #region Unity callbacks

        private void Start()
        {
            PlayController.Instance.OnShipSelect.AddListener(UpdateUi);
        }

        private void OnEnable()
        {
            UpdateUi();
        }

        #endregion Unity callbacks
    }
}