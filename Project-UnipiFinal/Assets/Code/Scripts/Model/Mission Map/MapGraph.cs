using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraph : DAG<MapNode>
{
    public List<List<MapNode>> NodeGroups { get; private set; }

    public MapGraph()
    {
        NodeGroups = new List<List<MapNode>>();
    }

    public void AddNodesGroup(List<MapNode> _nodesGroup)
    {
        NodeGroups.Add(_nodesGroup);

        foreach (MapNode node in _nodesGroup)
        {
            AddVertex(node);
        }
    }

    public void ConnectNodes(MapNode fromNode, MapNode toNode)
    {
        AddEdge(fromNode, toNode);
    }
}
