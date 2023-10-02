using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MissionMapPresenter
{
    MissionMapView _missionMapView;
    MissionLocalService _missionLocalService;
    LevelLocalService _levelLocalService;
    Mission _mission;

    public MissionMapPresenter(MissionMapView missionMapView)
    {
        _missionMapView = missionMapView;
        _missionLocalService = new MissionLocalService();
        _levelLocalService = new LevelLocalService();

        // Initialize events
        _missionMapView.OnViewInitialized.AddListener(HandleViewInitialized);
    }

    public async void HandleViewInitialized()
    {
        if (_mission != null)
            return;

        _mission = await _missionLocalService.LoadMission();

        if (_mission.MapGraph == null)
        {
            Debug.Log($"<color=blue>Generating MapGraph...</color>");
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
            PlayerPrefs.SetInt("IsFinalNode", 0);
            PlayerPrefs.SetInt("MissionScore", 0);
            PlayerPrefs.SetInt("MissionReputation", 0);
            PlayerPrefs.SetInt("MissionMatches", 0);

            CreateAndSaveLevelsOnConnectedNodesOfPointedNode(PlayerPrefs.GetInt("CurrentPointedNodeId"));

            await _missionLocalService.SaveMission(_mission);
        }

        _missionMapView.DisplayMap(_mission.MapGraph);
    }

    public void HandleAttackOnNode(MapNode node)
    {
        // if (node.NodeType != NodeType.Attack)
        //     return;

        if (node.NodeType == NodeType.Final)
            PlayerPrefs.SetInt("IsFinalNode", 1);

        // Save selected node id to be used later by Match Manager 
        PlayerPrefs.SetInt("SelectedNodeId", node.Id);

        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

    public void HandleContinueInNextPointedNode()
    {
        _missionMapView.DisplayPointedNode();

        CreateAndSaveLevelsOnConnectedNodesOfPointedNode(PlayerPrefs.GetInt("CurrentPointedNodeId"));
    }

    public async void AbandonMission()
    {
        await _missionLocalService.DeleteMission();

        _mission = null;

        GameManager.Instance.UpdateGameState(GameState.AbandoningMission);
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
        foreach (var nodeId in pointedNode.ConnectedNodes)
        {
            if (selectedNode.Id == nodeId)
                return true;
        }

        return false;
    }

    public async void CreateAndSaveLevelsOnConnectedNodesOfPointedNode(int pointedNodeId)
    {
        MapNode pointedNode = _mission.MapGraph.GetNodeById(pointedNodeId);
        MapNode connectedNode = null;
        List<Level> levelsList = new List<Level>();
        GameMode tempGameMode = GameMode.TimeAttack;
        foreach (var nodeId in pointedNode.ConnectedNodes)
        {
            connectedNode = _mission.MapGraph.GetNodeById(nodeId);
            if (connectedNode.NodeType == NodeType.Attack)
                tempGameMode = GameMode.TimeAttack;
            else if (connectedNode.NodeType == NodeType.BoostHub)
                tempGameMode = GameMode.MatchPoint;

            levelsList.Add(new Level()
            {
                NodeId = connectedNode.Id,
                Difficulty = GetDifficultyByNodeType(connectedNode),
                GameMode = tempGameMode,
            });
        }

        await _levelLocalService.SaveAllLevels(levelsList);

        // Nested method to define diffuculty
        string GetDifficultyByNodeType(MapNode mapNode)
        {
            if (mapNode.NodeType == NodeType.Final)
            {
                switch (_mission.Difficulty)
                {
                    case "Easy":
                        return "Medium";
                    case "Medium":
                        return "Hard";
                    case "Hard":
                        return "Very Hard";
                    case "Very Hard":
                        return "Very Hard";
                    default:
                        return "Medium";
                }
            }

            int randNum = Random.Range(0, 2);
            switch (_mission.Difficulty)
            {
                case "Easy":
                    return randNum == 0 ? "Easy" : "Medium";
                case "Medium":
                    return randNum == 0 ? "Medium" : "Hard";
                case "Hard":
                    return randNum == 0 ? "Hard" : "Very Hard";
                case "Very Hard":
                    return "Very Hard";
                default:
                    return "Medium";
            }
        }
    }
}