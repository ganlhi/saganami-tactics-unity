using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class CommandPanel : MonoBehaviour
    {
        #region Editor customization
#pragma warning disable 0649
        [SerializeField] private Transform content;
        [SerializeField] private GameObject plottingPanel;
        [SerializeField] private GameObject targettingPanel;
        [SerializeField] private TMP_Text shipName;
#pragma warning restore
        #endregion Editor customization

        #region Unity callbacks

        private void Start()
        {
            PlayController.Instance.OnStepStart.AddListener(DisplaySubPanelForStep);
            PlayController.Instance.OnShipSelect.AddListener(UpdateHeader);
            DisplaySubPanelForStep();
        }

        #endregion Unity callbacks

        #region Public methods

        public void ToggleFocusOnShip()
        {
            PlayController.Instance.ToggleFocusOnShip(PlayController.Instance.SelectedShip);
        }

        public void SelectNextShip()
        {
            var ships = PlayController.Instance.GetAllShips();
            var idx = (ships.IndexOf(PlayController.Instance.SelectedShip) + 1) % ships.Count;
            var ship = ships[idx];

            PlayController.Instance.SelectShip(ship);
        }

        public void SelectPreviousShip()
        {
            var ships = PlayController.Instance.GetAllShips();
            var idx = ships.IndexOf(PlayController.Instance.SelectedShip) - 1;
            if (idx < 0)
            {
                idx = ships.Count - 1;
            }

            var ship = ships[idx];

            PlayController.Instance.SelectShip(ship);
        }

        #endregion Public methods

        #region Private methods

        private void UpdateHeader()
        {
            var ship = PlayController.Instance.SelectedShip;
            shipName.text = ship.Name;
        }

        private void DisplaySubPanelForStep()
        {
            foreach (Transform child in content)
            {
                child.gameObject.SetActive(false);
            }

            var ship = PlayController.Instance.SelectedShip;
            if (ship != null && ship.photonView.Owner.IsLocal)
            {
                switch (PlayController.Instance.Step)
                {
                    case TurnStep.Plotting:
                        plottingPanel.SetActive(true);
                        break;

                    case TurnStep.SetupSalvos:
                        targettingPanel.SetActive(true);
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion Private methods
    }
}