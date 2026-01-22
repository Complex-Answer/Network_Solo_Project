using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LineDraw : MonoBehaviour
{
    [SerializeField] private float _drawLine = 5;
    public void DrawLine(RectTransform _rectTransform, List<NodeData>[] nodeConnection)
    {
        foreach (Transform child in _rectTransform)
        {
            if (child.name == "Line")
            {
                Destroy(child.gameObject);
            }
        }
        foreach (var layer in nodeConnection)
        {
            foreach (var parent in layer)
            {
                foreach (var child in parent.NextStairs)
                {
                    CreatLine(_rectTransform, parent.Position, child.Position);
                }
            }
        }
    }
    private void CreatLine(RectTransform rectTransform, Vector2 start, Vector2 end)
    {
        GameObject line = new($"Line", typeof(RectTransform), typeof(Image));
        line.transform.SetParent(rectTransform, false);
        line.transform.SetAsFirstSibling();

        Image img = line.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0.8f);

        RectTransform rect = line.GetComponent<RectTransform>();
        Vector2 dir = end - start;
        float distance = dir.magnitude;

        rect.sizeDelta = new Vector2(distance, _drawLine);
        rect.pivot = new Vector2(0, 0.5f);
        rect.anchoredPosition = start;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.rotation = Quaternion.Euler(0, 0, angle);
    }
}
