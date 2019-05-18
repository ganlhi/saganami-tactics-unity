using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class SliderValueText : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Slider sliderInput;
        [SerializeField] private TMPro.TMP_Text textUi;
#pragma warning restore

        private void Update()
        {
            textUi.text = sliderInput.value.ToString();
        }
    }
}