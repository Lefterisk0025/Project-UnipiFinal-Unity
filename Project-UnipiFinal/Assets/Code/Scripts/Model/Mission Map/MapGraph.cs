using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraph
{
    public List<List<MapNode>> NodeGroups { get; private set; }

    public MapGraph()
    {
        NodeGroups = new List<List<MapNode>>();
    }

    public void AddNodesGroup(List<MapNode> _nodesGroup)
    {
        NodeGroups.Add(_nodesGroup);
    }
}
