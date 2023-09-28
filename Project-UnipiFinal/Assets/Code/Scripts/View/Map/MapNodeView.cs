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

    bool _isPointed = false;

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
                HandleNodeSelection();
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
        Debug.Log($"<color=green>-------------- Node {Node.Id} - Type: {Node.NodeType} --------------</color>");
        foreach (var item in Node.ConnectedNodes)
        {
            Debug.Log($"<color=green>Child Node {item.Id} - Type: {item.NodeType}</color>");

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Node.IsActive && !_isPointed)
            NotifyObservers(Actions.SelectNode);
    }

    private void HandleCurrentObjectiveState()
    {
        _pointIcon.SetActive(true);
        _isPointed = true;
        Node.IsActive = false;
    }

    private void HandleNodeSelection()
    {
        _selectionIcon.SetActive(true);
        Vector3 tempScale = _selectionIcon.GetComponent<RectTransform>().localScale;
        LeanTween.scale(_selectionIcon, new Vector3(tempScale.x + 0.3f, tempScale.y + 0.3f, tempScale.z), 0.1f).setOnComplete(() =>
        {
            LeanTween.scale(_selectionIcon, new Vector3(tempScale.x, tempScale.y, tempScale.z), 0.1f);
        });
    }
}
