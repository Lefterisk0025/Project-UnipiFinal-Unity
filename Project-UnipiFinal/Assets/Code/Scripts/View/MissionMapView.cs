using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.AddressableAssets;

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

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
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
            GenerateRandomSeed();

            // Set map config based on difficulty
            MissionMapConfig tempConfig = null;
            if (mission.Difficulty == "Easy")
                tempConfig = GetMissionConfigBasedOnDifficulty(Difficulty.Easy);
            else if (mission.Difficulty == "Medium")
                tempConfig = GetMissionConfigBasedOnDifficulty(Difficulty.Medium);
            else if (mission.Difficulty == "Hard")
                tempConfig = GetMissionConfigBasedOnDifficulty(Difficulty.Hard);
            else if (mission.Difficulty == "Very Hard")
                tempConfig = GetMissionConfigBasedOnDifficulty(Difficulty.VeryHard);

            // Initialize map's position to the screen
            RectTransform contectRectTransform = _contentParent.gameObject.GetComponent<RectTransform>();
            contectRectTransform.localPosition = new Vector3(0, _contentParent.transform.position.y);

            // Choose random values based on intervals from map config
            int mapDepth = Random.Range(tempConfig.MapDepth.x, tempConfig.MapDepth.y + 1);
            // Ask presenter for map graph
            MapGraph mapGraph = _missionPresenter.CreateMissionMapGraph(mapDepth, tempConfig.MaxNodesPerVerticalLine);
            mission.MapGraph = mapGraph;

            // Update local data
            await _missionPresenter.UpdateLocalMissionData(mission);

            GenerateMissionMapGraphOnScene(mapGraph);
        }
        else
        {
            if (_contentParent.childCount == 0)
                GenerateMissionMapGraphOnScene(mission.MapGraph);
        }

    }

    private MissionMapConfig GetMissionConfigBasedOnDifficulty(Difficulty difficulty)
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
