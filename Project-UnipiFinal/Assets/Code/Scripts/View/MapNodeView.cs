using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeView : Subject, IPointerDownHandler
{
    public enum NodeState { Default, Selected, Completed }
    public MapNode Node { get; set; }

    [SerializeField] private GameObject SelectionIcon;

    private void Start()
    {
        UpdateView(NodeState.Default);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Node.IsActive)
            NotifyObservers(Actions.SelectNode);
    }

    public void UpdateView(NodeState nodeState)
    {
        switch (nodeState)
        {
            case NodeState.Default:
                SelectionIcon.SetActive(false);
                break;
            case NodeState.Selected:
                SelectionIcon.SetActive(true);
                break;
        }
    }
}
