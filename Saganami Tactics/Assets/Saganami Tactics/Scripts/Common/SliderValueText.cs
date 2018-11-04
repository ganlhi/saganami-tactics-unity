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

        private void Update()
        {
            textUi.text = sliderInput.value.ToString();
        }
    }
}