using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MissionMapPresenter
{
    MissionMapView _missionMapView;
    MissionLocalService _missionLocalService;
    Mission _mission;

    public MissionMapPresenter(MissionMapView missionMapView)
    {
        _missionMapView = missionMapView;

        _missionLocalService = new MissionLocalService();

        // Initialize events
        _missionMapView.OnViewInitialized.AddListener(HandleViewInitialized);
        _missionMapView.OnMapGenerated.AddListener(SetCurrentPointedNode);
    }

    public async void HandleViewInitialized()
    {
        if (_mission != null)
            return;

        _mission = await _missionLocalService.LoadMission();

        if (_mission.MapGraph == null)
        {
            MissionMapConfig tempConfig = null;
            if (_mission.Difficulty == "Easy")
                tempConfig = _missionMapView.GetMissionMapConfigBasedOnDifficulty(Difficulty.Easy);
            else if (_mission.Difficulty == "Medium")
                tempConfig = _missionMapView.GetMissionMapConfigBasedOnDifficulty(Difficulty.Medium);
            else if (_mission.Difficulty == "Hard")
                tempConfig = _missionMapView.GetMissionMapConfigBasedOnDifficulty(Difficulty.Hard);
            else if (_mission.Difficulty == "Very Hard")
                tempConfig = _missionMapView.GetMissionMapConfigBasedOnDifficulty(Difficulty.VeryHard);

            int mapDepth = Random.Range(tempConfig.MapDepth.x, tempConfig.MapDepth.y + 1);
            _mission.MapGraph = new MapGraph(mapDepth, tempConfig.MaxNodesPerLine);
            _mission.MapGraph.CreateRandomGraph();

            PlayerPrefs.SetInt("CurrentPointedNodeId", 0); // Root node has id=0 
            PlayerPrefs.SetInt("PreviousPointedNodeId", -1);
        }

        _missionMapView.DisplayMissionInfo(_mission);
        _missionMapView.DisplayMap(_mission.MapGraph);
    }

    public void HandleAttackOnNode(MapNode node)
    {
        if (node.NodeType != NodeType.Attack)
            return;

        // Save selected node id to be used later by Match Manager 
        PlayerPrefs.SetInt("SelectedNodeId", node.Id);

        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    public void AbandonMission()
    {
        _mission = null;
        _missionMapView.OnViewInitialized.RemoveListener(HandleViewInitialized);
    }

    public void SetCurrentPointedNode()
    {
        _missionMapView.DisplayPointedNode(PlayerPrefs.GetInt("CurrentPointedNodeId"));
    }

    public void SetSelectedNode(MapNode node)
    {
        var currPointedNode = _mission.MapGraph.GetNodeById(PlayerPrefs.GetInt("CurrentPointedNodeId"));

        if (!CanVisitSelectedNode(currPointedNode, node))
            return;

        _missionMapView.DisplaySelectedNode(node.Id);
    }

    private bool CanVisitSelectedNode(MapNode pointedNode, MapNode selectedNode)
    {
        foreach (var connectedNode in pointedNode.ConnectedNodes)
        {
            if (selectedNode.Id == connectedNode.Id)
                return true;
        }

        return false;
    }
}
