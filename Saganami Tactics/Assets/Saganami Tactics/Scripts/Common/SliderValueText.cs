using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class SliderValueText : MonoBehaviour
    {

        [SerializeField]
        private Slider sliderInput;

        [SerializeField]
        private TMPro.TMP_Text textUi;

        [SerializeField] private int multiplier = 1;

        private void Update()
        {
            textUi.text = (sliderInput.value * multiplier).ToString();
        }
    }
}