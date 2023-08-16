using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILineRenderer : MonoBehaviour
{
    public RectTransform canvasRectTransform;
    public Image lineImage;

    public void DrawLine(Vector2 screenPointA, Vector2 screenPointB, Transform parent)
    {
        Vector2 differenceVector = screenPointB - screenPointA;
        Vector2 middlePoint = screenPointA + (differenceVector / 2.0f);
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        float distance = differenceVector.magnitude;

        RectTransform lineRectTransform = Instantiate(lineImage, canvasRectTransform).rectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, middlePoint, null, out localPoint);
        lineRectTransform.anchoredPosition = localPoint;
        lineRectTransform.sizeDelta = new Vector2(distance, lineRectTransform.sizeDelta.y);
        lineRectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}