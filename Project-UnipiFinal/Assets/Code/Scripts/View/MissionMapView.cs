using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MissionMapView : MonoBehaviour, IObserver
{
    MissionPresenter _missionPresenter;

    [Header("General UI")]
    [SerializeField] private TextMeshProUGUI _missionTitleGUI;
    [SerializeField] private TextMeshProUGUI _missionDifficultyGUI;

    [Header("Map Settings")]
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _horizontalNodesContainerPrefab;
    [SerializeField] private MapNodeView _attackNodePrefab;
    [SerializeField] private MapNodeView _boostHubNodePrefab;
    [SerializeField] private List<MissionMapConfig> _missionMapConfigsList;

    [Header("Line Settings")]
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _linesParent;

    private IDictionary<MapNode, GameObject> _spawnedNodes; // Dict to connect MapNode objects with their scene Game Objects 
    private List<MapLineRenderer> _mapLinesList;
    private ScrollRect _scrollRect;
    private MapNodeView _selectedNodeView;
    private MapNodeView _currObjectiveNode;

    private void Awake()
    {
        _missionPresenter = new MissionPresenter(this);
        _spawnedNodes = new Dictionary<MapNode, GameObject>();
        _mapLinesList = new List<MapLineRenderer>();
        _scrollRect = GetComponentInChildren<ScrollRect>();

        if (_scrollRect != null)
        {
            _scrollRect.onValueChanged.AddListener(OnScrollMoved);
        }
    }

    private void OnEnable()
    {
        _missionPresenter.InitializeMission();

        // Initialize map's position to the screen
        RectTransform contectRectTransform = _contentParent.gameObject.GetComponent<RectTransform>();
        contectRectTransform.localPosition = new Vector3(0, _contentParent.transform.position.y);
    }

    public MissionMapConfig GetMissionMapConfigBasedOnDifficulty(Difficulty difficulty)
    {
        foreach (var config in _missionMapConfigsList)
        {
            if (config.Difficulty == difficulty)
            {
                return config;
            }
        }
        return null;
    }

    public void OnNotify(ISubject subject, Actions action)
    {
        switch (action)
        {
            case Actions.SelectNode:
                HandleNodeSelection((MapNodeView)subject);
                break;
        }
    }

    private void HandleNodeSelection(MapNodeView selectedMapNodeView)
    {
        if (!_missionPresenter.CanVisitSelectedNode(_currObjectiveNode.Node, selectedMapNodeView.Node))
            return;

        if (_selectedNodeView != null)
            _selectedNodeView.UpdateView(MapNodeView.NodeState.Default);

        _selectedNodeView = selectedMapNodeView;
        _selectedNodeView.UpdateView(MapNodeView.NodeState.Selected);
    }

    public async void InvokeAbandonMission()
    {
        foreach (Transform child in _contentParent)
        {
            Destroy(child.gameObject);
        }

        await _missionPresenter.DeleteLocalMissionData();

        GameManager.Instance.UpdateGameState(GameState.AbandoningMission);
    }

    public void InvokeAttack()
    {
        if (_selectedNodeView.Node.NodeType != NodeType.Attack)
            return;

        PlayerPrefs.SetInt("SelectedNodeId", _selectedNodeView.Node.Id);

        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    public void SetMissionUI(Mission mission)
    {
        _missionTitleGUI.text = "Mission: " + mission.Title;
        _missionDifficultyGUI.text = "Difficulty: " + mission.Difficulty.ToString();
    }

    public void GenerateMissionMapGraphOnScene(MapGraph mapGraph)
    {
        if (_contentParent.childCount > 0)
            return;

        MapNodeView mapNodeView;
        int id = 0;
        foreach (List<MapNode> nodesGroup in mapGraph.NodeGroups)
        {
            var verticalLine = Instantiate(_horizontalNodesContainerPrefab, _contentParent.transform);

            foreach (MapNode node in nodesGroup)
            {
                if (node.NodeType == NodeType.Begin || node.NodeType == NodeType.Attack || node.NodeType == NodeType.Default)
                    _spawnedNodes[node] = Instantiate(_attackNodePrefab, verticalLine.transform).gameObject;
                else
                    _spawnedNodes[node] = Instantiate(_boostHubNodePrefab, verticalLine.transform).gameObject;

                mapNodeView = _spawnedNodes[node].GetComponent<MapNodeView>();
                mapNodeView.Node = node; // Connect views with models
                mapNodeView.Node.Id = id;
                mapNodeView.AddObserver(this);

                id++;
            }
        }

        foreach (KeyValuePair<MapNode, List<MapNode>> entry in mapGraph.GetGraphAsAdjacencyList())
        {
            foreach (MapNode targetNode in entry.Value)
            {
                Debug.Log("Id: " + targetNode.Id + ", Type: " + targetNode.NodeType.ToString());
            }
        }

        StartCoroutine(DrawMapLines(mapGraph));
    }

    private IEnumerator DrawMapLines(MapGraph mapGraph)
    {
        yield return new WaitForSeconds(1);

        GameObject newLine = null;
        MapLineRenderer mapLineRenderer = null;

        var graph = mapGraph.GetGraphAsAdjacencyList();

        foreach (KeyValuePair<MapNode, List<MapNode>> entry in graph)
        {
            GameObject originMapNodeGO = _spawnedNodes[entry.Key];
            GameObject targetMapNodeGO = null;

            foreach (MapNode targetNode in entry.Value)
            {
                targetMapNodeGO = _spawnedNodes[targetNode];

                newLine = Instantiate(_linePrefab, _linesParent);
                newLine.transform.SetSiblingIndex(0);

                mapLineRenderer = newLine.GetComponent<MapLineRenderer>();
                mapLineRenderer.SetLineRenderer(originMapNodeGO.transform, targetMapNodeGO.transform);
                mapLineRenderer.UpdateLinePosition(_canvas);

                _mapLinesList.Add(mapLineRenderer);
            }
        }
    }

    private void OnScrollMoved(Vector2 position)
    {
        foreach (var line in _mapLinesList)
        {
            line.UpdateLinePosition(_canvas);
        }
    }

    public void DisplayCurrentSelectedObjectiveNode(MapNode mapNode)
    {
        _currObjectiveNode = _spawnedNodes[mapNode].gameObject.GetComponent<MapNodeView>();
        _currObjectiveNode.UpdateView(MapNodeView.NodeState.CurrentObjective);
    }
}
