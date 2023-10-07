using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLineRenderer : MonoBehaviour
{
    private Transform _worldPointA;
    private Transform _worldPointB;
    private RectTransform _rectTransform;

    public void SetLineRenderer(Transform pointA, Transform pointB)
    {
        _worldPointA = pointA;
        _worldPointB = pointB;

        _rectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    public void UpdateLinePosition(Canvas canvas)
    {
        // Convert world positions to screen space
        Vector2 screenPosA = RectTransformUtility.WorldToScreenPoint(Camera.main, _worldPointA.position);
        Vector2 screenPosB = RectTransformUtility.WorldToScreenPoint(Camera.main, _worldPointB.position);

        // Convert screen positions to positions relative to the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosA, canvas.worldCamera, out Vector2 canvasPosA);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosB, canvas.worldCamera, out Vector2 canvasPosB);

        // Calculate direction
        Vector2 direction = canvasPosB - canvasPosA;
        float distance = direction.magnitude;
        direction.Normalize();

        // Set the pivot to the start of the line
        _rectTransform.pivot = new Vector2(0, 0.5f);

        // Position the line GameObject between the two points
        _rectTransform.anchoredPosition = canvasPosA;

        // Set the rotation to point towards the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        // Scale the line to stretch between the two points
        _rectTransform.sizeDelta = new Vector2(distance, _rectTransform.sizeDelta.y);
    }
}
