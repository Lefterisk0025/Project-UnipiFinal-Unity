using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Events;


public class MissionMapView : MonoBehaviour, IObserver
{
    MissionMapPresenter _missionMapPresenter;

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

    private List<GameObject> _nodeGameObjectsList;
    private List<MapLineRenderer> _mapLinesList;
    private ScrollRect _scrollRect;
    private MapNodeView _selectedNodeView;
    private MapNodeView _pointedNodeView;
    int tempCurrentPointedNodeId = 0;

    [HideInInspector] public UnityEvent OnViewInitialized;
    [HideInInspector] public UnityEvent OnViewDisabled;
    [HideInInspector] public UnityEvent OnMapGenerated;

    private void Awake()
    {
        _missionMapPresenter = new MissionMapPresenter(this);
        _nodeGameObjectsList = new List<GameObject>();
        _mapLinesList = new List<MapLineRenderer>();
        _scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void OnEnable()
    {
        OnViewInitialized.Invoke();
    }

    private void OnDisable()
    {
        OnViewDisabled.Invoke();
    }

    public void OnNotify(ISubject subject, Actions action)
    {
        switch (action)
        {
            case Actions.SelectNode:
                OnNodeSelected((MapNodeView)subject);
                break;
        }
    }

    private void OnScrollMoved(Vector2 position)
    {
        foreach (var line in _mapLinesList)
        {
            line.UpdateLinePosition(_canvas);
        }
    }

    public void OnAbandonButtonClicked()
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

        _nodeGameObjectsList.Clear();
        _mapLinesList.Clear();

        _missionMapPresenter.AbandonMission();

        GameManager.Instance.UpdateGameState(GameState.AbandoningMission);
    }

    public void OnNodeSelected(MapNodeView nodeView)
    {
        _missionMapPresenter.SetSelectedNode(nodeView.Node);
    }

    public void OnAttackButtonClicked()
    {
        if (_selectedNodeView == null)
            return;

        _missionMapPresenter.HandleAttackOnNode(_selectedNodeView.Node);
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

    public void DisplayMissionInfo(Mission mission)
    {
        _missionTitleGUI.text = "Mission: " + mission.Title;
        _missionDifficultyGUI.text = "Difficulty: " + mission.Difficulty.ToString();
    }

    public void DisplayMap(MapGraph mapGraph)
    {
        Debug.Log("Display Map called");
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

                _nodeGameObjectsList.Add(spawnedNode);

                mapNodeView = spawnedNode.GetComponent<MapNodeView>();
                mapNodeView.Node = node; // Connect views (gameobjects) with models
                mapNodeView.AddObserver(this);
            }
        }

        StartCoroutine(DrawMapLines(mapGraph));
    }

    private IEnumerator DrawMapLines(MapGraph mapGraph)
    {
        yield return new WaitForSeconds(2);

        GameObject newLine = null;
        MapLineRenderer mapLineRenderer = null;

        foreach (var nodeGroup in mapGraph.NodeGroups)
        {
            foreach (var node in nodeGroup)
            {
                GameObject originMapNodeGO = GetNodeGameObjectById(node.Id);
                GameObject targetMapNodeGO = null;

                foreach (MapNode targetNode in node.ConnectedNodes)
                {
                    targetMapNodeGO = GetNodeGameObjectById(targetNode.Id);

                    newLine = Instantiate(_linePrefab, _linesParent);
                    newLine.transform.SetSiblingIndex(0);

                    mapLineRenderer = newLine.GetComponent<MapLineRenderer>();
                    mapLineRenderer.SetLineRenderer(originMapNodeGO.transform, targetMapNodeGO.transform);
                    mapLineRenderer.UpdateLinePosition(_canvas);

                    _mapLinesList.Add(mapLineRenderer);
                }
            }
        }

        if (_scrollRect != null)
            _scrollRect.onValueChanged.AddListener(OnScrollMoved);

        OnMapGenerated.Invoke();
    }

    public void DisplayPointedNode(int nodeId)
    {
        // Reset previous pointed node
        if (_pointedNodeView != null)
            _pointedNodeView.UpdateView(MapNodeView.NodeState.Default);
        // Set the new one
        _pointedNodeView = GetNodeGameObjectById(nodeId).GetComponent<MapNodeView>();
        _pointedNodeView.UpdateView(MapNodeView.NodeState.CurrentObjective);
    }

    public void DisplaySelectedNode(int nodeId)
    {
        // Reset previous selected node
        if (_selectedNodeView != null)
            _selectedNodeView.UpdateView(MapNodeView.NodeState.Default);
        // Set the new one
        _selectedNodeView = GetNodeGameObjectById(nodeId).GetComponent<MapNodeView>();
        _selectedNodeView.UpdateView(MapNodeView.NodeState.Selected);
    }

    private GameObject GetNodeGameObjectById(int nodeId)
    {
        foreach (var nodeGO in _nodeGameObjectsList)
        {
            var mapNodeView = nodeGO.GetComponent<MapNodeView>();
            if (mapNodeView.Node.Id == nodeId)
                return nodeGO;
        }

        return null;
    }
}
