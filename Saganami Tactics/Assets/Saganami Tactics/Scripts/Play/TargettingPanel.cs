using UnityEngine;

namespace ST
{
    public class TargettingPanel : MonoBehaviour
    {
        #region Editor customization

        [SerializeField]
        private GameObject targetSelectorAft;

        [SerializeField]
        private GameObject targetSelectorForward;

        [SerializeField]
        private GameObject targetSelectorPort;

        [SerializeField]
        private GameObject targetSelectorStarboard;

        #endregion Editor customization



        #region Public methods

        public void SetSalvo(int salvo)
        {
            Salvo targetSalvo;
            switch (salvo)
            {
                case 0:
                    targetSalvo = Salvo.Early;
                    break;

                case 1:
                    targetSalvo = Salvo.Middle;
                    break;

                case 2:
                    targetSalvo = Salvo.Late;
                    break;

                default:
                    return;
            }

            PlayController.Instance.TargettingSalvo = targetSalvo;
        }

        public void StopTargetting()
        {
            PlayController.Instance.TargettingFrom = null;

            targetSelectorAft.SetActive(true);
            targetSelectorForward.SetActive(true);
            targetSelectorPort.SetActive(true);
            targetSelectorStarboard.SetActive(true);
        }

        public void StartTargettingForward()
        {
            PlayController.Instance.TargettingFrom = Side.Forward;

            targetSelectorAft.SetActive(false);
            targetSelectorForward.SetActive(true);
            targetSelectorPort.SetActive(false);
            targetSelectorStarboard.SetActive(false);
        }

        public void StartTargettingAft()
        {
            PlayController.Instance.TargettingFrom = Side.Aft;

            targetSelectorAft.SetActive(true);
            targetSelectorForward.SetActive(false);
            targetSelectorPort.SetActive(false);
            targetSelectorStarboard.SetActive(false);
        }

        public void StartTargettingPort()
        {
            PlayController.Instance.TargettingFrom = Side.Port;

            targetSelectorAft.SetActive(false);
            targetSelectorForward.SetActive(false);
            targetSelectorPort.SetActive(true);
            targetSelectorStarboard.SetActive(false);
        }

        public void StartTargettingStarboard()
        {
            PlayController.Instance.TargettingFrom = Side.Starboard;

            targetSelectorAft.SetActive(false);
            targetSelectorForward.SetActive(false);
            targetSelectorPort.SetActive(false);
            targetSelectorStarboard.SetActive(true);
        }

        #endregion Public methods

        #region Unity callbacks

        private void OnEnable()
        {
        }

        #endregion Unity callbacks
    }
}