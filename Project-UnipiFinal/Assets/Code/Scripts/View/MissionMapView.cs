using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Build.Pipeline.Tasks;

public class MissionMapView : MonoBehaviour
{
    MissionMapPresenter _missionMapPresenter;

    [Header("General UI")]
    [SerializeField] private TextMeshProUGUI _missionTitleGUI;
    [SerializeField] private TextMeshProUGUI _missionDifficultyGUI;

    [Header("Map Settings")]
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _verticalNodesContainerPrefab;
    [SerializeField] private NodeView _attackNodePrefab;
    [SerializeField] private NodeView _boostHubNodePrefab;
    [SerializeField] private UILineRenderer _uiLineRenderer;

    [SerializeField] private int mapDepth;
    [SerializeField] private int maxNodesPerVerticalLine;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform canvasRectTransform;

    private IDictionary<MapNode, GameObject> _spawnedNodes;


    private void Awake()
    {
        _missionMapPresenter = new MissionMapPresenter(this);
        _spawnedNodes = new Dictionary<MapNode, GameObject>();
    }

    private void OnEnable()
    {
        Mission mission = _missionMapPresenter.GetLocalSavedMission();

        _missionTitleGUI.text = "Mission: " + mission.Title;
        _missionDifficultyGUI.text = "Difficulty: " + mission.Difficulty.ToString();

        GenerateMap();
    }

    public void AbandonMission()
    {
        GameManager.Instance.UpdateGameState(GameState.AbandoningMission);
    }

    private void GenerateMap()
    {
        MapGraph map = _missionMapPresenter.CreateMapGraph(mapDepth, maxNodesPerVerticalLine);

        foreach (List<MapNode> nodesGroup in map.NodeGroups)
        {
            var verticalLine = Instantiate(_verticalNodesContainerPrefab, _contentParent.transform);

            foreach (MapNode node in nodesGroup)
            {
                if (node.NodeType == NodeType.Begin || node.NodeType == NodeType.Attack)
                {
                    var spawnedNode = Instantiate(_attackNodePrefab, verticalLine.transform);
                    _spawnedNodes[node] = spawnedNode.gameObject;
                }
                else
                {
                    var spawnedNode = Instantiate(_boostHubNodePrefab, verticalLine.transform);
                    _spawnedNodes[node] = spawnedNode.gameObject;
                }
            }
        }

        // Draw edges between nodes
        Dictionary<MapNode, List<MapNode>> mapGraph = _missionMapPresenter.GetMapGraph();
        foreach (KeyValuePair<MapNode, List<MapNode>> node in mapGraph)
        {
            foreach (var toNode in node.Value)
            {
                Vector2 fromNodePos = _spawnedNodes[node.Key].transform.position;
                Vector2 toNodePos = _spawnedNodes[toNode].transform.position;
                _uiLineRenderer.DrawLine(fromNodePos, toNodePos, _contentParent.transform);
            }
        }

        // Show it on screen
    }
}
