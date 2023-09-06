using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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
    [SerializeField] private int mapDepth;
    [SerializeField] private int maxNodesPerVerticalLine;

    [Header("Line Settings")]
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _linesParent;

    private IDictionary<MapNode, GameObject> _spawnedNodes; // Connect MapNode objects with their scene Game Objects 
    private List<MapLineRenderer> _mapLinesList;
    private MapNodeView _selectedNode;

    bool canUpdateLinesPosition;

    private void Awake()
    {
        _missionPresenter = new MissionPresenter(this);
        _spawnedNodes = new Dictionary<MapNode, GameObject>();
        _mapLinesList = new List<MapLineRenderer>();
    }

    private void Update()
    {
        if (canUpdateLinesPosition)
        {
            foreach (var mapLine in _mapLinesList)
            {
                Debug.Log("Updating...");
                mapLine.UpdateLinePosition(_canvas);
            }
        }
    }

    private async void OnEnable()
    {
        //Mission mission = await _missionPresenter.GetLocalSavedMission();
        Mission mission = new Mission() { Title = "A New Dawn", Description = "Something realy good is happening in the house of the rising sun.", Difficulty = "Medium" };

        _missionTitleGUI.text = "Mission: " + mission.Title;
        _missionDifficultyGUI.text = "Difficulty: " + mission.Difficulty.ToString();

        if (_contentParent.childCount > 0)
            return;

        if (mission.MapGraph == null)
        {
            RectTransform contectRectTransform = _contentParent.gameObject.GetComponent<RectTransform>();
            contectRectTransform.localPosition = new Vector3(0, _contentParent.transform.position.y);

            // Check mission's difficulty to define mapDepth and maxNodesPerVerticalLine

            MapGraph mapGraph = _missionPresenter.CreateMissionMapGraph(mapDepth, maxNodesPerVerticalLine);
            mission.MapGraph = mapGraph;

            await _missionPresenter.UpdateLocalMissionData(mission);

            GenerateMissionMapGraphOnScene(mapGraph);
        }
        else
        {
            if (_contentParent.childCount == 0)
                GenerateMissionMapGraphOnScene(mission.MapGraph);
        }

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
        if (_selectedNode != null)
            _selectedNode.UpdateView(MapNodeView.NodeState.Default);

        _selectedNode = selectedMapNodeView;
        _selectedNode.UpdateView(MapNodeView.NodeState.Selected);
    }

    public async void AbandonMission()
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
        if (_selectedNode.Node.NodeType != NodeType.Attack)
            return;

        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    private void GenerateMissionMapGraphOnScene(MapGraph mapGraph)
    {
        MapNodeView mapNodeView;
        foreach (List<MapNode> nodesGroup in mapGraph.NodeGroups)
        {
            var verticalLine = Instantiate(_horizontalNodesContainerPrefab, _contentParent.transform);

            foreach (MapNode node in nodesGroup)
            {
                if (node.NodeType == NodeType.Begin || node.NodeType == NodeType.Attack)
                    _spawnedNodes[node] = Instantiate(_attackNodePrefab, verticalLine.transform).gameObject;
                else
                    _spawnedNodes[node] = Instantiate(_boostHubNodePrefab, verticalLine.transform).gameObject;

                mapNodeView = _spawnedNodes[node].GetComponent<MapNodeView>();
                mapNodeView.Node = node;
                mapNodeView.AddObserver(this);
            }
        }

        StartCoroutine(DrawMapLines(mapGraph));
    }

    private IEnumerator DrawMapLines(MapGraph mapGraph)
    {
        yield return new WaitForSeconds(1);

        GameObject newLine = null;
        RectTransform rectTransform = null;
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

                rectTransform = newLine.GetComponent<RectTransform>();
                mapLineRenderer = newLine.GetComponent<MapLineRenderer>();

                mapLineRenderer.SetLineRenderer(originMapNodeGO.transform, targetMapNodeGO.transform);
                mapLineRenderer.UpdateLinePosition(_canvas);

                _mapLinesList.Add(mapLineRenderer);
            }
        }

        canUpdateLinesPosition = true;
    }
}
