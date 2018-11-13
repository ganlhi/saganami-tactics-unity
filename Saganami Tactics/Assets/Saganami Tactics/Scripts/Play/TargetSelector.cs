using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ST
{
    public class TargetSelector : MonoBehaviour
    {
        #region Editor customization

        [SerializeField]
        private TMP_Text targetName;

        [SerializeField]
        private TMP_Text missiles;

        [SerializeField]
        private TMP_Text distance;

        [SerializeField]
        private GameObject content;

        #endregion Editor customization

        #region Private variables

        private TargetData target;
        private bool targetting;

        #endregion Private variables

        #region Public variables

        public TargetData Target
        {
            get { return target; }
            set
            {
                target = value;
                content.SetActive(true);

                targetName.text = target.Target.Name;
                distance.text = target.Distance.ToString();
                missiles.text = target.Missiles.ToString();
            }
        }

        public bool Targetting
        {
            get { return targetting; }
            set
            {
                targetting = value;
                if (targetting)
                {
                    OnTargetModeOn.Invoke();
                }
                else
                {
                    OnTargetModeOff.Invoke();
                }
            }
        }

        public UnityEvent OnTargetModeOn = new UnityEvent();
        public UnityEvent OnTargetModeOff = new UnityEvent();

        #endregion Public variables

        #region Unity callbacks

        private void Update()
        {
        }

        #endregion Unity callbacks
    }
}