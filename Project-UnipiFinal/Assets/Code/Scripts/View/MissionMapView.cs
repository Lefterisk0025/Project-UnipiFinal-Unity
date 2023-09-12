using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private List<GameObject> _spawnedNodesList; // Dict to connect MapNode objects with their scene Game Objects 
    private List<MapLineRenderer> _mapLinesList;
    private ScrollRect _scrollRect;
    public MapNodeView _selectedNodeView;
    public MapNodeView _currPointedNodeView;

    int tempCurrentPointedNodeId = 0;

    private void Awake()
    {
        _missionPresenter = new MissionPresenter(this);
        _spawnedNodesList = new List<GameObject>();
        _mapLinesList = new List<MapLineRenderer>();
        _scrollRect = GetComponentInChildren<ScrollRect>();

        if (_scrollRect != null)
        {
            _scrollRect.onValueChanged.AddListener(OnScrollMoved);
        }
    }

    private void OnEnable()
    {
        if (_contentParent.childCount > 0)
            return;

        _missionPresenter.InitializeMission();

        // Initialize map's position to the screen
        RectTransform contectRectTransform = _contentParent.gameObject.GetComponent<RectTransform>();
        contectRectTransform.localPosition = new Vector3(0, _contentParent.transform.position.y);

        if (_selectedNodeView != null)
        {
            _selectedNodeView.UpdateView(MapNodeView.NodeState.Default);
            _selectedNodeView = null;
        }
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
        if (!_missionPresenter.CanVisitSelectedNode(_currPointedNodeView.Node.Id, selectedMapNodeView.Node.Id))
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

        foreach (Transform child in _linesParent)
        {
            if (child.name.Contains("LinePrefab"))
                Destroy(child.gameObject);
        }

        _spawnedNodesList.Clear();
        _mapLinesList.Clear();

        await _missionPresenter.DeleteLocalMissionData();

        GameManager.Instance.UpdateGameState(GameState.AbandoningMission);
    }

    public void InvokeAttack()
    {
        // if (_selectedNodeView.Node.NodeType != NodeType.Attack)
        //     return;

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
        MapNodeView mapNodeView;
        foreach (List<MapNode> nodesGroup in mapGraph.NodeGroups)
        {
            var verticalLine = Instantiate(_horizontalNodesContainerPrefab, _contentParent.transform);

            foreach (MapNode node in nodesGroup)
            {
                GameObject spawnedNode = null;
                if (node.NodeType == NodeType.Begin || node.NodeType == NodeType.Attack || node.NodeType == NodeType.Default)
                    spawnedNode = Instantiate(_attackNodePrefab, verticalLine.transform).gameObject;
                else
                    spawnedNode = Instantiate(_boostHubNodePrefab, verticalLine.transform).gameObject;

                _spawnedNodesList.Add(spawnedNode);

                mapNodeView = spawnedNode.GetComponent<MapNodeView>();
                mapNodeView.Node = node; // Connect views (gameobjects) with models
                mapNodeView.AddObserver(this);
            }
        }

        StartCoroutine(DrawMapLines(mapGraph));
    }

    private IEnumerator DrawMapLines(MapGraph mapGraph)
    {
        yield return new WaitForSeconds(1);

        GameObject newLine = null;
        MapLineRenderer mapLineRenderer = null;

        foreach (var nodeGroup in mapGraph.NodeGroups)
        {
            foreach (var node in nodeGroup)
            {
                GameObject originMapNodeGO = GetNodeGameObjectMyMapNodeId(node.Id);
                GameObject targetMapNodeGO = null;

                foreach (MapNode targetNode in node.ConnectedNodes)
                {
                    targetMapNodeGO = GetNodeGameObjectMyMapNodeId(targetNode.Id);

                    newLine = Instantiate(_linePrefab, _linesParent);
                    newLine.transform.SetSiblingIndex(0);

                    mapLineRenderer = newLine.GetComponent<MapLineRenderer>();
                    mapLineRenderer.SetLineRenderer(originMapNodeGO.transform, targetMapNodeGO.transform);
                    mapLineRenderer.UpdateLinePosition(_canvas);

                    _mapLinesList.Add(mapLineRenderer);
                }
            }
        }

        DisplayCurrentPointedNode(GetCurrentPointedNodeId());
    }

    private void OnScrollMoved(Vector2 position)
    {
        foreach (var line in _mapLinesList)
        {
            line.UpdateLinePosition(_canvas);
        }
    }

    public async void DisplayCurrentPointedNode(int nodeId)
    {
        _currPointedNodeView = GetNodeGameObjectMyMapNodeId(nodeId).GetComponent<MapNodeView>();
        _currPointedNodeView.UpdateView(MapNodeView.NodeState.CurrentObjective);

        if (await _missionPresenter.SaveObjectivesOfConnectedNodes(_currPointedNodeView.Node))
        {
            Debug.Log($"<color=green>Objectives saved successfully!</color>");
        }
        else
            Debug.Log($"<color=red>An error occured while saving objectives!</color>");
    }

    private GameObject GetNodeGameObjectMyMapNodeId(int id)
    {
        foreach (var nodeGO in _spawnedNodesList)
        {
            var mapNodeView = nodeGO.GetComponent<MapNodeView>();
            if (mapNodeView.Node.Id == id)
                return nodeGO;
        }

        return null;
    }

    public void SetCurrentPointedNodeId(int mapNodeId)
    {
        PlayerPrefs.SetInt("CurrentPointedNodeId", mapNodeId);
    }

    public int GetCurrentPointedNodeId()
    {
        return PlayerPrefs.GetInt("CurrentPointedNodeId");
    }
}
