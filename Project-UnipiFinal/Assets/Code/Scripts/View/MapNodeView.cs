using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeView : Subject, IPointerDownHandler
{
    public enum NodeState { Default, Selected, CurrentObjective, Completed }

    public MapNode Node { get; set; }

    [SerializeField] private GameObject _selectionIcon;
    [SerializeField] private GameObject _pointIcon;

    bool isPointed = false;

    private void Awake()
    {
        UpdateView(NodeState.Default);

        _pointIcon.SetActive(false);
        _selectionIcon.SetActive(false);
    }

    public void UpdateView(NodeState nodeState)
    {
        switch (nodeState)
        {
            case NodeState.Default:
                _selectionIcon.SetActive(false);
                _pointIcon.SetActive(false);
                break;
            case NodeState.Selected:
                _selectionIcon.SetActive(true);
                break;
            case NodeState.CurrentObjective:
                HandleCurrentObjectiveState();
                break;
        }
    }

    public void PrintPosition()
    {
        Debug.Log(this.transform.position);
    }

    public void PrintNodeInfo()
    {
        Debug.Log($"<color=green>-------------- Node {Node.Id} - Type: {Node.NodeType.ToString()} --------------</color>");
        foreach (var item in Node.ConnectedNodes)
        {
            Debug.Log($"<color=green>Child Node {item.Id} - Type: {item.NodeType.ToString()}</color>");

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Node.IsActive && !isPointed)
            NotifyObservers(Actions.SelectNode);
    }

    private void HandleCurrentObjectiveState()
    {
        _pointIcon.SetActive(true);
        isPointed = true;
    }
}
