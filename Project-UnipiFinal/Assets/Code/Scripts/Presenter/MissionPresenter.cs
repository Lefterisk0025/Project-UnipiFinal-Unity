using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region LOCAL CRUD

    public Mission GetLocalSavedMission()
    {
        return _missionService.GetLocalMissionData();
    }

    public void CreateLocalMissionData(Mission mission)
    {
        if (_missionService.SaveLocalMissionData(mission))
        {
            Debug.Log("Data saved successfully!");
        }
    }

    public bool UpdateLocalMissionData(Mission mission)
    {
        if (_missionService.SaveLocalMissionData(mission))
            return true;

        return false;
    }

    public bool DeleteLocalMissionData()
    {
        if (_missionService.DeleteLocalMission())
            return true;

        return false;
    }

    #endregion

    public List<Mission> GetNewRandomMissions(int count)
    {
        return _missionService.GetNewRandomMissions(count);
    }

    public List<Mission> GetCurrentMissions()
    {
        return null;
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
            Debug.Log("i: " + i);

            var tempGroup = new List<MapNode>();

            // Check for root or final node
            if (i == 0 || i == (mapDepth - 1))
            {
                tempGroup.Add(new MapNode(NodeType.Begin));
                mapGraph.AddNodesGroup(tempGroup);
                continue;
            }

            // Select random number of nodes per vertical line (group)
            int randGroupSize = Random.Range(1, maxNodesPerVerticalLine + 1);
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

        Debug.Log("Node Groups: " + mapGraph.NodeGroups);

        return mapGraph;
    }
}
