using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeView : Subject, IPointerDownHandler
{
    public enum NodeState { Default, Selected, Pointed, Completed }
    public MapNode Node { get; set; }

    [SerializeField] private GameObject _selectionIcon;
    [SerializeField] private GameObject _pointIcon;

    private void Start()
    {
        UpdateView(NodeState.Default);

        _pointIcon.SetActive(false);
        _selectionIcon.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (Node.IsActive)
        //     NotifyObservers(Actions.SelectNode);
    }

    public void UpdateView(NodeState nodeState)
    {
        switch (nodeState)
        {
            case NodeState.Default:
                _selectionIcon.SetActive(false);
                break;
            case NodeState.Selected:
                _selectionIcon.SetActive(true);
                break;
            case NodeState.Pointed:
                _pointIcon.SetActive(true);
                break;
        }
    }
}
