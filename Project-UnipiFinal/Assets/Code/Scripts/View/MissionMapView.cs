using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Build.Pipeline.Tasks;

public class MissionMapView : MonoBehaviour, IObserver
{
    MissionPresenter _missionPresenter;

    [Header("General UI")]
    [SerializeField] private TextMeshProUGUI _missionTitleGUI;
    [SerializeField] private TextMeshProUGUI _missionDifficultyGUI;

    [Header("Map Settings")]
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _verticalNodesContainerPrefab;
    [SerializeField] private MapNodeView _attackNodePrefab;
    [SerializeField] private MapNodeView _boostHubNodePrefab;

    [SerializeField] private int mapDepth;
    [SerializeField] private int maxNodesPerVerticalLine;

    private IDictionary<MapNode, GameObject> _spawnedNodes; // Connect MapNode objects with their scene Game Objects 
    private MapNodeView _selectedNode;

    private void Awake()
    {
        _missionPresenter = new MissionPresenter(this);
        _spawnedNodes = new Dictionary<MapNode, GameObject>();
    }

    private void OnEnable()
    {
        Mission mission = _missionPresenter.GetLocalSavedMission();

        _missionTitleGUI.text = "Mission: " + mission.Title;
        _missionDifficultyGUI.text = "Difficulty: " + mission.Difficulty.ToString();

        if (_contentParent.childCount > 0)
            return;

        if (mission.MapGraph == null)
        {
            RectTransform contectRectTransform = _contentParent.gameObject.GetComponent<RectTransform>();
            contectRectTransform.localPosition = new Vector3(0, _contentParent.transform.position.y);

            MapGraph mapGraph = _missionPresenter.CreateMissionMapGraph(mapDepth, maxNodesPerVerticalLine);
            mission.MapGraph = mapGraph;

            _missionPresenter.UpdateLocalMissionData(mission);

            GenerateGraphMap(mapGraph);
        }
        else
        {
            if (_contentParent.childCount == 0)
                GenerateGraphMap(mission.MapGraph);
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

    public void AbandonMission()
    {
        foreach (Transform child in _contentParent)
        {
            Destroy(child.gameObject);
        }

        _missionPresenter.DeleteLocalMissionData();

        GameManager.Instance.UpdateGameState(GameState.AbandoningMission);
    }

    public void InvokeAttack()
    {
        if (_selectedNode.Node.NodeType != NodeType.Attack)
            return;

        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    private void GenerateGraphMap(MapGraph mapGraph)
    {
        MapNodeView mapNodeView;
        foreach (List<MapNode> nodesGroup in mapGraph.NodeGroups)
        {
            var verticalLine = Instantiate(_verticalNodesContainerPrefab, _contentParent.transform);

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
    }

    public void ClearMap()
    {
        foreach (Transform child in _contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
