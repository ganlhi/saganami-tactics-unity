using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class SelectionMarker : MonoBehaviour
    {
        #region Editor customization
#pragma warning disable 0649
        [SerializeField] private GameObject markerImages;
        [SerializeField] private TMP_Text markerText;
        [SerializeField] private int padding;
#pragma warning restore
        #endregion Editor customization

        #region Unity callbacks

        public GameObject SelectedObject;
        public string SelectedObjectName;

        private void Update()
        {
            if (SelectedObject != null)
            {
                markerImages.SetActive(true);
                markerText.enabled = true;

                markerText.text = SelectedObjectName;

                Vector3[] corners = new Vector3[8];
                var bounds = SelectedObject.GetComponent<Collider>().bounds;
                var cam = Camera.main;

                var cx = bounds.center.x;
                var cy = bounds.center.y;
                var cz = bounds.center.z;
                var ex = bounds.extents.x;
                var ey = bounds.extents.y;
                var ez = bounds.extents.z;

                corners[0] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz + ez));
                corners[1] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz - ez));
                corners[2] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz + ez));
                corners[3] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz - ez));
                corners[4] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz + ez));
                corners[5] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz - ez));
                corners[6] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz + ez));
                corners[7] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz - ez));

                float min_x = corners[0].x;
                float min_y = corners[0].y;
                float max_x = corners[0].x;
                float max_y = corners[0].y;

                for (int i = 1; i < 8; i++)
                {
                    if (corners[i].x < min_x)
                    {
                        min_x = corners[i].x;
                    }
                    if (corners[i].y < min_y)
                    {
                        min_y = corners[i].y;
                    }
                    if (corners[i].x > max_x)
                    {
                        max_x = corners[i].x;
                    }
                    if (corners[i].y > max_y)
                    {
                        max_y = corners[i].y;
                    }
                }

                Rect visualRect = Rect.MinMaxRect(min_x - padding, min_y - padding, max_x + padding, max_y + padding);

                if (visualRect.width < 80)
                {
                    var diff = 80 - visualRect.width;
                    visualRect.xMin -= diff / 2;
                    visualRect.xMax += diff / 2;
                }

                if (visualRect.height < 80)
                {
                    var diff = 80 - visualRect.height;
                    visualRect.yMin -= diff / 2;
                    visualRect.yMax += diff / 2;
                }

                RectTransform rt = GetComponent<RectTransform>();

                rt.position = new Vector2(visualRect.xMin, visualRect.yMin);
                rt.sizeDelta = new Vector2(visualRect.width, visualRect.height);
            }
            else
            {
                markerImages.SetActive(false);
                markerText.enabled = false;
            }
        }

        #endregion Unity callbacks
    }
}