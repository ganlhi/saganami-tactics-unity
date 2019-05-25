using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class ButtonsPosition : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] SetupController ctrl;
        [SerializeField] Button buttonForward;
        [SerializeField] Button buttonBackward;
        [SerializeField] Button buttonLeft;
        [SerializeField] Button buttonRight;
        [SerializeField] Button buttonUp;
        [SerializeField] Button buttonDown;
#pragma warning restore

        void FixedUpdate()
        {
            buttonForward.interactable = ctrl.CanGoForward();
            buttonBackward.interactable = ctrl.CanGoBackward();
            buttonLeft.interactable = ctrl.CanGoLeft();
            buttonRight.interactable = ctrl.CanGoRight();
            buttonUp.interactable = ctrl.CanGoUp();
            buttonDown.interactable = ctrl.CanGoDown();
        }
    }
}