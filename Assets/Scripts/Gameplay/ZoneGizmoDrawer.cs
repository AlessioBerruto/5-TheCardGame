using UnityEngine;

[ExecuteAlways]
public class ZoneGizmoDrawer : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private Vector2 fallbackSize = new Vector2(110f, 158f);
    [SerializeField] private bool showInPlayMode = false;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || showInPlayMode)
        {
            Gizmos.color = gizmoColor;

            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);

                Gizmos.DrawLine(corners[0], corners[1]);
                Gizmos.DrawLine(corners[1], corners[2]);
                Gizmos.DrawLine(corners[2], corners[3]);
                Gizmos.DrawLine(corners[3], corners[0]);
            }
            else
            {
                Vector3 pos = transform.position;
                Vector3 size = new Vector3(fallbackSize.x, fallbackSize.y, 0f);
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }
}
