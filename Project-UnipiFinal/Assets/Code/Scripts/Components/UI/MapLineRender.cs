using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLineRender : MonoBehaviour
{
    public RectTransform firstUIElement;
    public RectTransform secondUIElement;
    public Image line;

    public Transform parent;

    public void DrawLine()
    {
        Vector2 startScreenPos = firstUIElement.anchoredPosition;
        Vector2 endScreenPos = secondUIElement.anchoredPosition;

        Vector2 direction = (endScreenPos - startScreenPos).normalized;
        float distance = Vector2.Distance(startScreenPos, endScreenPos);

        line.rectTransform.sizeDelta = new Vector2(distance, 1);  // Assuming you want a 1-pixel wide line
        line.rectTransform.anchoredPosition = startScreenPos;
        line.rectTransform.pivot = new Vector2(0, 0.5f);  // Makes sure line extends to the right from start position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        line.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
