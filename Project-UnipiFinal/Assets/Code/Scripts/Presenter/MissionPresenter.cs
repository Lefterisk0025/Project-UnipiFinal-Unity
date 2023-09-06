using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Random = UnityEngine.Random;

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
            // In second group (i == 1), map can't have a single node
            int randGroupSize = 0;
            if (i == 1)
                randGroupSize = Random.Range(2, maxNodesPerVerticalLine + 1);
            else
                randGroupSize = Random.Range(1, maxNodesPerVerticalLine + 1);

            int randNode = 0;

            // Check for avoiding same number of nodes per neighboring groups
            while (randGroupSize == prevSize)
            {
                randGroupSize = Random.Range(1, maxNodesPerVerticalLine + 1);
            }

            for (int j = 0; j < randGroupSize; j++)
            {
                // Choose random node
                randNode = Random.Range(0, 2);
                MapNode tempNode;

                if (randNode == 0)
                    tempNode = new MapNode(NodeType.BoostHub);
                else
                    tempNode = new MapNode(NodeType.Attack);

                tempGroup.Add(tempNode);
            }

            mapGraph.AddNodesGroup(tempGroup);
            prevSize = randGroupSize;
        }

        // Connect Nodes between Node Groups
        // Iterate node groups
        for (int i = 0; i < mapGraph.NodeGroups.Count; i++)
        {
            // Connect the root node with every node in the 2nd group
            if (i == 0)
            {
                var rootNode = mapGraph.NodeGroups[0][0];
                foreach (var node in mapGraph.NodeGroups[1])
                {
                    mapGraph.ConnectNodes(rootNode, node);
                }
            }


        }

        return mapGraph;
    }

    public bool AreNodesNeighboring(MapNode node1, MapNode node2)
    {
        return false;
    }
}
