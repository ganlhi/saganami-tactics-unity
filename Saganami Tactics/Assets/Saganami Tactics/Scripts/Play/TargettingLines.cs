using System.Collections.Generic;
using UnityEngine;

namespace ST
{
    public class TargettingLines : MonoBehaviour
    {
        #region Editor customization

        [SerializeField]
        private GameObject linePrefab;

        #endregion Editor customization

        #region Public methods

        public void RemoveLines()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void ShowLines(List<TargetData> targets)
        {
            // Delete existing lines
            RemoveLines();

            // Create one line for each target
            foreach (var target in targets)
            {
                var lr = Instantiate(linePrefab, transform).GetComponent<LineRenderer>();

                Transform from, to;

                switch (target.Salvo)
                {
                    case Salvo.Early:
                        from = target.Attacker.transform;
                        to = target.Target.transform;
                        break;

                    case Salvo.Middle:
                        from = target.Attacker.transform;
                        to = target.Target.MiddleMarker.transform;
                        break;

                    case Salvo.Late:
                        from = target.Attacker.MiddleMarker.transform;
                        to = target.Target.EndMarker.transform;
                        break;

                    default:
                        continue;
                }

                lr.SetPosition(0, from.position);
                lr.SetPosition(1, to.position);
            }
        }

        #endregion Public methods

        #region Unity callbacks

        private void Start()
        {
        }

        private void Update()
        {
        }

        #endregion Unity callbacks
    }
}