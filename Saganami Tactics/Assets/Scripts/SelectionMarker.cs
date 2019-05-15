using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class SelectionMarker : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Image markerImage;
    [SerializeField] private TMP_Text markerText;
    [SerializeField] private int padding;
#pragma warning restore

    private void Update()
    {
        if (GameController.Instance.SelectedShip != null)
        {
            markerImage.enabled = true;
            markerText.enabled = true;

            markerText.text = GameController.Instance.SelectedShip.Name;

            Vector3[] corners = new Vector3[8];
            var bounds = GameController.Instance.SelectedShip.GetComponent<Collider>().bounds;
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

            RectTransform rt = GetComponent<RectTransform>();

            rt.position = new Vector2(visualRect.xMin, visualRect.yMin);
            rt.sizeDelta = new Vector2(visualRect.width, visualRect.height);
        }
        else
        {
            markerImage.enabled = false;
            markerText.enabled = false;
        }
    }
}