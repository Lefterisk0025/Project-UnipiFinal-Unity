using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class MissionPresenter
{
    MissionMapView _missionMapView;
    MissionsCacheView _missionsCacheView;
    MissionService _missionService;
    ObjectiveService _objectiveService;
    Mission _mission;

    public MissionPresenter(MissionMapView missionMapView)
    {
        _missionMapView = missionMapView;
        _missionService = new MissionService();
        _objectiveService = new ObjectiveService();
        //_mission = new Mission();
    }

    public MissionPresenter()
    {
        _missionService = new MissionService();
    }

    #region LOCAL DATA

    // ----------------- MISSION -----------------

    public async Task<Mission> GetLocalSavedMission()
    {
        return await _missionService.GetLocalMissionData();
    }

    public async Task<bool> SaveLocalMissionData(Mission mission)
    {
        if (await _missionService.SaveLocalMissionData(mission))
            return true;

        return false;
    }

    public async Task<bool> DeleteLocalMissionData()
    {
        if (await _missionService.DeleteLocalMission())
            return true;

        return false;
    }

    #endregion

    #region REMOTE FETCH


    #endregion

    #region MAP NODE OPERATIONS

    public MapNode GetRootMapNode()
    {
        return _mission.MapGraph.NodeGroups[0][0];
    }

    public async Task<bool> CreateAndSaveObjectivesOfConnectedNodes(MapNode mapNode)
    {
        var connectedNodes = mapNode.ConnectedNodes;

        List<Objective> objectivesList = new List<Objective>();
        Objective objective;
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            objective = new Objective();
            objective.Id = i;
            objective.MapNodeId = connectedNodes[i].Id;
            objective.Difficulty = _mission.Difficulty;

            int randGameMode = Random.Range(0, 2); // 0 or 1
            if (randGameMode == 0)
                objective.GameMode = GameMode.TimeAttack;
            else
                objective.GameMode = GameMode.MatchPoint;

            objectivesList.Add(objective);
        }

        if (await _objectiveService.SaveLocalObjectivesListData(objectivesList))
            return true;
        else
            return false;
    }

    #endregion

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
    }

    public async void InitializeMission()
    {
        _mission = await GetLocalSavedMission();

        //_mission = new Mission() { Title = "A New Dawn", Description = "Something realy good is happening in the house of the rising sun.", Difficulty = "Hard" };

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

            // Choose random values based on intervals from map config
            int mapDepth = Random.Range(tempConfig.MapDepth.x, tempConfig.MapDepth.y + 1);
            _mission.MapGraph = CreateMissionMapGraph(mapDepth, tempConfig.MaxNodesPerVerticalLine);

            _missionMapView.SetCurrentPointedNodeId(GetRootMapNode().Id);

            // Update local data
            await SaveLocalMissionData(_mission);
        }

        _missionMapView.SetMissionUI(_mission);
        _missionMapView.GenerateMissionMapGraphOnScene(_mission.MapGraph);
    }

    public MapGraph CreateMissionMapGraph(int mapDepth, int maxNodesPerVerticalLine)
    {
        _mission.MapGraph = new MapGraph();

        GenerateRandomSeed();

        int prevSize = 0;
        int id = 0;
        // Create node groups
        for (int i = 0; i < mapDepth; i++)
        {
            var tempGroup = new List<MapNode>();

            // Check for root or final node
            if (i == 0 || i == (mapDepth - 1))
            {
                MapNode mapNode = new MapNode(NodeType.Begin);
                mapNode.Id = id;
                tempGroup.Add(mapNode);
                _mission.MapGraph.AddNodesGroup(tempGroup);

                id++;
                continue;
            }

            // Select random number of nodes per vertical line (group)
            // In the second group or the one before final (i == 1 or i == mapDepth-2), map can't have a single node
            int randGroupSize = 0;
            if (i == 1 || i == (mapDepth - 1))
                randGroupSize = Random.Range(2, maxNodesPerVerticalLine + 1);
            else
                randGroupSize = Random.Range(1, maxNodesPerVerticalLine + 1);

            // Check for avoiding same number of nodes per neighboring groups
            while (randGroupSize == prevSize)
            {
                randGroupSize = Random.Range(1, maxNodesPerVerticalLine + 1);
            }

            // Initialize empty nodes in the node groups
            for (int j = 0; j < randGroupSize; j++)
            {
                MapNode mapNode = new MapNode(NodeType.Attack);
                mapNode.Id = id;
                tempGroup.Add(mapNode);
                id++;
            }

            _mission.MapGraph.AddNodesGroup(tempGroup);
            prevSize = randGroupSize;
        }

        // Place BoostHub Nodes
        int nodesAfter = 0; // number that indicates the number of nodes to be passed before placing new ones, can be 2 or 3
        for (int i = 2; i < _mission.MapGraph.NodeGroups.Count - 1; i++)
        {
            // The node group before the final group must always contains BoostHub nodes
            if (i == _mission.MapGraph.NodeGroups.Count - 2)
            {
                if (_mission.MapGraph.NodeGroups[i].Count == 1)
                    _mission.MapGraph.NodeGroups[i][0].NodeType = NodeType.BoostHub;

                // All nodes, except one, at this particular group must be BoostHubs
                int nodeToBeExcluded = Random.Range(0, _mission.MapGraph.NodeGroups[i].Count);
                for (int j = 0; j < _mission.MapGraph.NodeGroups[i].Count; j++)
                {
                    if (j == nodeToBeExcluded)
                        continue;

                    _mission.MapGraph.NodeGroups[i][j].NodeType = NodeType.BoostHub;
                }
            }

            // Decide the number of BoostHub nodes in the chosen group
            int numOfBHNodesToBePlaced = 0;
            if (_mission.MapGraph.NodeGroups[i].Count == 2)
                numOfBHNodesToBePlaced = 1;
            else if (_mission.MapGraph.NodeGroups[i].Count == 3 || _mission.MapGraph.NodeGroups[i].Count == 4)
                numOfBHNodesToBePlaced = 2;

            // Decide the indexes of the nodes that wont't become BoostHubs in a group
            HashSet<int> indexes = new HashSet<int>();
            while (indexes.Count < numOfBHNodesToBePlaced)
            {
                int num = Random.Range(0, _mission.MapGraph.NodeGroups[i].Count);
                indexes.Add(num);
            }

            // Place BoostHubNodes
            for (int j = 0; j < _mission.MapGraph.NodeGroups[i].Count; j++)
            {
                if (indexes.Contains(j))
                    continue;

                _mission.MapGraph.NodeGroups[i][j].NodeType = NodeType.BoostHub;
            }

            nodesAfter = Random.Range(2, 3);
            i += nodesAfter; // 2 or 3 nodes after to place the
        }

        // Connect Nodes between Node Groups
        // Iterate node groups
        for (int i = 0; i < mapDepth; i++)
        {
            // Check for the root node
            // Connect the root node with every node in the 2nd group
            if (i == 0)
            {
                var rootNode = _mission.MapGraph.NodeGroups[0][0];
                foreach (var node in _mission.MapGraph.NodeGroups[1])
                {
                    rootNode.ConnectedNodes.Add(node);
                }
                continue;
            }

            // Check for the destination node
            // Connect the last node for every node the previous group
            if (i == (mapDepth - 2))
            {
                var lastNode = _mission.MapGraph.NodeGroups[i + 1][0];
                foreach (var node in _mission.MapGraph.NodeGroups[i])
                {
                    lastNode.ConnectedNodes.Add(node);
                }
                break;
            }

            var currNodeGroup = _mission.MapGraph.NodeGroups[i];
            var nextNodeGroup = _mission.MapGraph.NodeGroups[i + 1];

            // When the next group has only one node, it must be connected with every node on the current group
            if (currNodeGroup.Count == 1)
            {
                foreach (var nextNode in nextNodeGroup)
                {
                    Debug.Log("Node connected to single node!");
                    currNodeGroup[0].ConnectedNodes.Add(nextNode);
                }
                continue;
            }

            if (nextNodeGroup.Count == 1)
            {
                foreach (var currNode in currNodeGroup)
                {
                    Debug.Log("Node connected to single node!");
                    currNode.ConnectedNodes.Add(nextNodeGroup[0]);
                }
                continue;
            }

            int randNum = 0;
            if (currNodeGroup.Count == 2)
            {
                // Always connect the two upper edge nodes 
                currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[0]);
                // Always connect the two bottom edge nodes 
                currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[nextNodeGroup.Count - 1]);

                if (nextNodeGroup.Count == 3)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[1]);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1]);
                        continue;
                    }
                    currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[1]);
                }

                if (nextNodeGroup.Count == 4)
                {
                    for (int j = 0; j < currNodeGroup.Count; j++)
                    {
                        // Node with index 0 in the current group
                        randNum = Random.Range(1, 4); // 1 or 2 or 3
                        if (randNum == 3)
                        {
                            currNodeGroup[j].ConnectedNodes.Add(nextNodeGroup[1]);
                            currNodeGroup[j].ConnectedNodes.Add(nextNodeGroup[2]);
                            continue;
                        }
                        currNodeGroup[j].ConnectedNodes.Add(nextNodeGroup[randNum]);
                    }
                }
            }

            if (currNodeGroup.Count == 3)
            {
                // Always connect the two upper edge nodes 
                currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[0]);
                // Always connect the two bottom edge nodes 
                currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[nextNodeGroup.Count - 1]);

                if (nextNodeGroup.Count == 2)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[0]);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1]);
                        continue;
                    }
                    currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum]);
                }

                if (nextNodeGroup.Count == 4)
                {
                    bool isNodeWithIndex1Connected = false;
                    bool isNodeWithIndex2Connected = false;
                    // For node with index 0 in the current group
                    randNum = Random.Range(0, 2); // 0 or 1
                    if (randNum == 1)
                    {
                        currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[1]);
                        isNodeWithIndex1Connected = true;
                    }

                    // For node with index 1 in the current group
                    randNum = Random.Range(1, 4); // 1 or 2 or 3
                    if (randNum == 3)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1]);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[2]);
                        isNodeWithIndex1Connected = true;
                        isNodeWithIndex2Connected = true;
                    }
                    currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum]);

                    // For node with index 2 in the current group
                    randNum = Random.Range(0, 2); // 0 or 1
                    if (randNum == 1)
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[2]);
                        isNodeWithIndex2Connected = true;
                    }

                    if (!isNodeWithIndex1Connected)
                    {
                        randNum = Random.Range(0, 2); // 0 or 1
                        currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[1]);
                    }

                    if (!isNodeWithIndex2Connected)
                    {
                        randNum = Random.Range(1, 3); // 1 or 2
                        currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[2]);
                    }

                }
            }

            if (currNodeGroup.Count == 4)
            {
                // Always connect the two upper edge nodes 
                currNodeGroup[0].ConnectedNodes.Add(nextNodeGroup[0]);
                // Always connect the two bottom edge nodes 
                currNodeGroup[3].ConnectedNodes.Add(nextNodeGroup[nextNodeGroup.Count - 1]);

                if (nextNodeGroup.Count == 2)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[0]);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1]);
                    }
                    else
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum]);
                    }


                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[0]);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1]);
                    }
                    else
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[randNum]);
                    }
                }

                if (nextNodeGroup.Count == 3)
                {
                    bool isTheMiddleNodeConnected = false;
                    // For node with index 1 in the current group
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[0]);
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[1]);
                        isTheMiddleNodeConnected = true;
                    }
                    else
                    {
                        currNodeGroup[1].ConnectedNodes.Add(nextNodeGroup[randNum]);
                        if (randNum == 1)
                            isTheMiddleNodeConnected = true;
                    }

                    // For node with index 2 in the current group
                    randNum = Random.Range(1, 4); // 1 or 2 or 3
                    if (randNum == 3)
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[1]);
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[2]);
                        isTheMiddleNodeConnected = true;
                    }
                    else
                    {
                        currNodeGroup[2].ConnectedNodes.Add(nextNodeGroup[randNum]);
                        if (randNum == 1)
                            isTheMiddleNodeConnected = true;
                    }

                    if (!isTheMiddleNodeConnected)
                    {
                        randNum = Random.Range(1, 3); // 1 or 2
                        currNodeGroup[randNum].ConnectedNodes.Add(nextNodeGroup[1]);
                    }
                }
            }
        }

        return _mission.MapGraph;
    }

    public MapNode GetMapNodeById(int id)
    {
        foreach (var nodeGroup in _mission.MapGraph.NodeGroups)
        {
            foreach (var node in nodeGroup)
            {
                if (node.Id == id)
                {
                    return node;
                }
            }
        }

        return null;
    }

    public bool CanVisitSelectedNode(int pointedNodeId, int selectedNodeId)
    {
        var pointedNode = GetMapNodeById(pointedNodeId);
        var selectedNode = GetMapNodeById(selectedNodeId);

        foreach (var connectedNode in pointedNode.ConnectedNodes)
        {
            if (selectedNode.Id == connectedNode.Id)
                return true;
        }

        return false;
    }
}
