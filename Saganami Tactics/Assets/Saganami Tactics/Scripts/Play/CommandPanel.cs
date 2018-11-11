using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class CommandPanel : MonoBehaviour
    {
        #region Editor customization

        [SerializeField]
        private Transform content;

        [SerializeField]
        private GameObject plottingPanel;

        [SerializeField]
        private Image flag;

        [SerializeField]
        private TMP_Text shipName;

        [SerializeField]
        private TMP_Text classAndCategory;

        #endregion Editor customization

        #region Unity callbacks

        private void Start()
        {
            PlayController.Instance.OnStepStart.AddListener(DisplaySubPanelForStep);
            PlayController.Instance.OnShipSelect.AddListener(UpdateHeader);
            DisplaySubPanelForStep();
        }

        #endregion Unity callbacks

        #region Private methods

        private void UpdateHeader()
        {
            var ship = PlayController.Instance.SelectedShip;
            flag.sprite = ship.SSD.Faction.Flag;
            shipName.text = ship.Name;
            classAndCategory.text = string.Format("{0} ({1})", ship.SSD.ClassName, ship.SSD.Category.Code);
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

                    default:
                        break;
                }
            }
        }

        #endregion Private methods
    }
}