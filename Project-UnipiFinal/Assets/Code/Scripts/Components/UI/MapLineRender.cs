using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MapLineRender : MonoBehaviour
{
    public Transform worldPointA; // Assign the first GameObject's Transform
    public Transform worldPointB; // Assign the second GameObject's Transform
    public Canvas canvas; // Assign your Screen Space - Overlay Canvas
    private RectTransform rectTransform;
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private Transform _parent;

    private void Update()
    {
        DrawLine();
    }

    public void DrawLine()
    {
        GameObject newLine = Instantiate(_linePrefab, canvas.transform);
        RectTransform rectTransform = newLine.GetComponent<RectTransform>();

        // Convert world positions to screen space
        Vector2 screenPosA = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPointA.position);
        Vector2 screenPosB = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPointB.position);

        // Convert screen positions to positions relative to the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosA, canvas.worldCamera, out Vector2 canvasPosA);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosB, canvas.worldCamera, out Vector2 canvasPosB);

        // Calculate direction
        Vector2 direction = canvasPosB - canvasPosA;
        float distance = direction.magnitude;
        direction.Normalize();

        // Set the pivot to the start of the line
        rectTransform.pivot = new Vector2(0, 0.5f);

        // Position the line GameObject between the two points
        rectTransform.anchoredPosition = canvasPosA;

        // Set the rotation to point towards the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        // Scale the line to stretch between the two points
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
    }
}
