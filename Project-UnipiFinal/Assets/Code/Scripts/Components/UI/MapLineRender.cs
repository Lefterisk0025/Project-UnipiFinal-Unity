using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLineRender : MonoBehaviour
{
    [SerializeField] private List<GameObject> _nodes;
    [SerializeField] private Camera mainCamera;

    UILineRenderer _lr;

    private void Start()
    {
        _lr = GetComponent<UILineRenderer>();

        Vector2 pos1 = _nodes[0].transform.position;
        Vector2 pos2 = _nodes[1].transform.position;

        _lr.DrawLine(pos1, pos2, transform);
    }
}
