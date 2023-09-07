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
    Mission _mission;

    public MissionPresenter(MissionMapView missionMapView)
    {
        _missionMapView = missionMapView;
        _missionService = new MissionService();
    }

    public MissionPresenter(MissionsCacheView missionsCacheView)
    {
        _missionsCacheView = missionsCacheView;
        _missionService = new MissionService();
    }

    #region LOCAL DATA

    // ----------------- MISSION -----------------

    public async Task<Mission> GetLocalSavedMission()
    {
        return await _missionService.GetLocalMissionData();
    }

    public async void SaveLocalMissionData(Mission mission)
    {
        if (await _missionService.SaveLocalMissionData(mission))
        {
            Debug.Log("Data saved successfully!");
        }
    }

    public async Task<bool> UpdateLocalMissionData(Mission mission)
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

    // ----------------- MISSIONS CACHE -----------------

    public async Task<List<Mission>> GetLocalMissionsCacheData()
    {
        return await _missionService.GetLocalMissionsCacheData();
    }

    public async void SaveLocalMissionsCacheData(List<Mission> missionsCache)
    {
        if (await _missionService.SaveLocalMissionsCacheData(missionsCache))
        {
            Debug.Log("Data saved successfully!");
        }
    }

    public async Task<bool> UpdateLocalMissionsCacheData(List<Mission> missionsCache)
    {
        if (await _missionService.SaveLocalMissionsCacheData(missionsCache))
            return true;

        return false;
    }

    public async Task<bool> DeleteLocalMissionsCacheData()
    {
        if (await _missionService.DeleteLocalMissionCacheData())
            return true;

        return false;
    }

    // FOR TESTING
    public List<Mission> GetRandomLocalMissions(int count)
    {
        return _missionService.GetRandomLocalMissions(count);
    }

    #endregion

    #region REMOTE FETCH

    public async Task<List<Mission>> GetRandomRemoteMissions(int count)
    {
        return await _missionService.GetRandomRemoteMissions(count);
    }

    #endregion

    public bool CanFetchNewMissions(DateTime lastFetchDateTime, DateTime currDateTime)
    {
        if (lastFetchDateTime == currDateTime)
            return true;

        // Check if 2 hours has passed since the last fetch time
        return (lastFetchDateTime - currDateTime).TotalHours >= 2.0;
    }

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
    }

    public MapGraph CreateMissionMapGraph(int mapDepth, int maxNodesPerVerticalLine)
    {
        MapGraph mapGraph = new MapGraph();

        GenerateRandomSeed();

        int prevSize = 0;

        // Create node groups
        for (int i = 0; i < mapDepth; i++)
        {
            var tempGroup = new List<MapNode>();

            // Check for root or final node
            if (i == 0 || i == (mapDepth - 1))
            {
                tempGroup.Add(new MapNode(NodeType.Begin));
                mapGraph.AddNodesGroup(tempGroup);
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
                tempGroup.Add(new MapNode(NodeType.Attack));
            }

            mapGraph.AddNodesGroup(tempGroup);
            prevSize = randGroupSize;
        }

        // Place BoostHub Nodes
        int nodesAfter = 0; // number that indicates the number of nodes to be passed before placing new ones, can be 2 or 3
        for (int i = 2; i < mapGraph.NodeGroups.Count - 1; i++)
        {
            // The node group before the final group must always contains BoostHub nodes
            if (i == mapGraph.NodeGroups.Count - 2)
            {
                if (mapGraph.NodeGroups[i].Count == 1)
                    mapGraph.NodeGroups[i][0].NodeType = NodeType.BoostHub;

                // All nodes, except one, at this particular group must be BoostHubs
                int nodeToBeExcluded = Random.Range(0, mapGraph.NodeGroups[i].Count);
                for (int j = 0; j < mapGraph.NodeGroups[i].Count; j++)
                {
                    if (j == nodeToBeExcluded)
                        continue;

                    mapGraph.NodeGroups[i][j].NodeType = NodeType.BoostHub;
                }
            }

            // Decide the number of BoostHub nodes in the chosen group
            int numOfBHNodesToBePlaced = 0;
            if (mapGraph.NodeGroups[i].Count == 2)
                numOfBHNodesToBePlaced = 1;
            else if (mapGraph.NodeGroups[i].Count == 3 || mapGraph.NodeGroups[i].Count == 4)
                numOfBHNodesToBePlaced = 2;

            // Decide the indexes of the nodes that wont't become BoostHubs in a group
            HashSet<int> indexes = new HashSet<int>();
            while (indexes.Count < numOfBHNodesToBePlaced)
            {
                int num = Random.Range(0, mapGraph.NodeGroups[i].Count);
                indexes.Add(num);
            }

            // Place BoostHubNodes
            for (int j = 0; j < mapGraph.NodeGroups[i].Count; j++)
            {
                if (indexes.Contains(j))
                    continue;

                mapGraph.NodeGroups[i][j].NodeType = NodeType.BoostHub;
            }

            nodesAfter = Random.Range(2, 3);
            i += nodesAfter; // 2 or 3 nodes after to place the
        }

        // Connect Nodes between Node Groups
        // Iterate node groups
        for (int i = 0; i < mapDepth; i++)
        {
            // Connect the root node with every node in the 2nd group
            if (i == 0)
            {
                var rootNode = mapGraph.NodeGroups[0][0];
                foreach (var node in mapGraph.NodeGroups[1])
                {
                    mapGraph.ConnectNodes(rootNode, node);
                }
                continue;
            }

            // Connect the last node for every node the previous group
            if (i == (mapDepth - 2))
            {
                var lastNode = mapGraph.NodeGroups[i + 1][0];
                foreach (var node in mapGraph.NodeGroups[i])
                {
                    mapGraph.ConnectNodes(lastNode, node);
                }
                break;
            }

            var currNodeGroup = mapGraph.NodeGroups[i];
            var nextNodeGroup = mapGraph.NodeGroups[i + 1];

            // When the current group has only one node, it must be connected with every node on the next group
            if (currNodeGroup.Count == 1)
            {
                var currNode = currNodeGroup[0];
                foreach (var targetNode in nextNodeGroup)
                {
                    mapGraph.ConnectNodes(currNode, targetNode);
                }

                continue;
            }

            // When the next group has only one node, it must be connected with every node on the current group
            if (nextNodeGroup.Count == 1)
            {
                var currNextNode = nextNodeGroup[0];
                foreach (var targetNode in currNodeGroup)
                {
                    mapGraph.ConnectNodes(currNextNode, targetNode);
                }

                continue;
            }

            int randNum = 0;
            if (currNodeGroup.Count == 2)
            {
                // Always connect the two upper edge nodes 
                mapGraph.ConnectNodes(currNodeGroup[0], nextNodeGroup[0]);
                // Always connect the two bottom edge nodes 
                mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[nextNodeGroup.Count - 1]);

                if (nextNodeGroup.Count == 3)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[0], nextNodeGroup[1]);
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[1]);
                        continue;
                    }
                    mapGraph.ConnectNodes(currNodeGroup[randNum], nextNodeGroup[1]);
                }

                if (nextNodeGroup.Count == 4)
                {
                    for (int j = 0; j < currNodeGroup.Count; j++)
                    {
                        // Node with index 0 in the current group
                        randNum = Random.Range(1, 4); // 1 or 2 or 3
                        if (randNum == 3)
                        {
                            mapGraph.ConnectNodes(currNodeGroup[j], nextNodeGroup[1]);
                            mapGraph.ConnectNodes(currNodeGroup[j], nextNodeGroup[2]);
                            continue;
                        }
                        mapGraph.ConnectNodes(currNodeGroup[j], nextNodeGroup[randNum]);
                    }
                }
            }

            if (currNodeGroup.Count == 3)
            {
                // Always connect the two upper edge nodes 
                mapGraph.ConnectNodes(currNodeGroup[0], nextNodeGroup[0]);
                // Always connect the two bottom edge nodes 
                mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[nextNodeGroup.Count - 1]);

                if (nextNodeGroup.Count == 2)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[0]);
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[1]);
                        continue;
                    }
                    mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[randNum]);
                }

                if (nextNodeGroup.Count == 4)
                {
                    bool isNodeWithIndex1Connected = false;
                    bool isNodeWithIndex2Connected = false;
                    // For node with index 0 in the current group
                    randNum = Random.Range(0, 2); // 0 or 1
                    if (randNum == 1)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[0], nextNodeGroup[1]);
                        isNodeWithIndex1Connected = true;
                    }

                    // For node with index 1 in the current group
                    randNum = Random.Range(1, 4); // 1 or 2 or 3
                    if (randNum == 3)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[1]);
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[2]);
                        isNodeWithIndex1Connected = true;
                        isNodeWithIndex2Connected = true;
                    }
                    mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[randNum]);

                    // For node with index 2 in the current group
                    randNum = Random.Range(0, 2); // 0 or 1
                    if (randNum == 1)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[2]);
                        isNodeWithIndex2Connected = true;
                    }

                    if (!isNodeWithIndex1Connected)
                    {
                        randNum = Random.Range(0, 2); // 0 or 1
                        mapGraph.ConnectNodes(currNodeGroup[randNum], nextNodeGroup[1]);
                    }

                    if (!isNodeWithIndex2Connected)
                    {
                        randNum = Random.Range(1, 3); // 1 or 2
                        mapGraph.ConnectNodes(currNodeGroup[randNum], nextNodeGroup[2]);
                    }

                }
            }

            if (currNodeGroup.Count == 4)
            {

                // Always connect the two upper edge nodes 
                mapGraph.ConnectNodes(currNodeGroup[0], nextNodeGroup[0]);
                // Always connect the two bottom edge nodes 
                mapGraph.ConnectNodes(currNodeGroup[3], nextNodeGroup[nextNodeGroup.Count - 1]);

                if (nextNodeGroup.Count == 2)
                {
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[0]);
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[1]);
                    }
                    else
                    {
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[randNum]);
                    }


                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[0]);
                        mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[1]);
                    }
                    else
                    {
                        mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[randNum]);
                    }
                }

                if (nextNodeGroup.Count == 3)
                {
                    bool isTheMiddleNodeConnected = false;
                    // For node with index 1 in the current group
                    randNum = Random.Range(0, 3); // 0 or 1 or 2
                    if (randNum == 2)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[0]);
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[1]);
                        isTheMiddleNodeConnected = true;
                    }
                    else
                    {
                        mapGraph.ConnectNodes(currNodeGroup[1], nextNodeGroup[randNum]);
                        if (randNum == 1)
                            isTheMiddleNodeConnected = true;
                    }

                    // For node with index 2 in the current group
                    randNum = Random.Range(1, 4); // 1 or 2 or 3
                    if (randNum == 3)
                    {
                        mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[1]);
                        mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[2]);
                        isTheMiddleNodeConnected = true;
                    }
                    else
                    {
                        mapGraph.ConnectNodes(currNodeGroup[2], nextNodeGroup[randNum]);
                        if (randNum == 1)
                            isTheMiddleNodeConnected = true;
                    }

                    if (!isTheMiddleNodeConnected)
                    {
                        randNum = Random.Range(1, 3); // 1 or 2
                        mapGraph.ConnectNodes(currNodeGroup[randNum], nextNodeGroup[1]);
                    }
                }
            }
        }

        return mapGraph;
    }

    public bool CanVisitSelectedNode(MapNode objectiveNode, MapNode selectedNode)
    {
        //var connectedNodes =

        return false;
    }
}
