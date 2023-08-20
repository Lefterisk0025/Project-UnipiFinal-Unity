using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MissionMapPresenter
{
    MissionMapView _missionMapView;
    MissionMapService _missionMapService;
    MapGraph _mapGraph;

    public MissionMapPresenter(MissionMapView missionMapView)
    {
        _missionMapView = missionMapView;

        _missionMapService = new MissionMapService();

        _mapGraph = new MapGraph();
    }

    public Mission GetLocalSavedMission()
    {
        return _missionMapService.GetLocalSavedMission();
    }

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
    }

    public Dictionary<MapNode, List<MapNode>> GetMapGraph()
    {
        return _mapGraph.GetGraph();
    }

    public MapGraph CreateMapGraph(int mapDepth, int maxNodesPerVerticalLine)
    {
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
                _mapGraph.AddNodesGroup(tempGroup);
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

            _mapGraph.AddNodesGroup(tempGroup);
            prevSize = randGroupSize;
        }

        // Connect Nodes between Node Groups
        for (int i = 0; i < _mapGraph.NodeGroups.Count; i++)
        {
            // Connect the root node with every node in the 2nd group
            if (i == 0)
            {
                var rootNode = _mapGraph.NodeGroups[0][0];
                foreach (var node in _mapGraph.NodeGroups[1])
                {
                    _mapGraph.ConnectNodes(rootNode, node);
                }
            }
        }

        return _mapGraph;
    }
}
