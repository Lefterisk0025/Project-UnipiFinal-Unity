using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public int Id { get; set; }
    public NodeType NodeType { get; set; }
    public bool IsActive { get; set; }
    public List<int> ConnectedNodes { get; set; }

    public MapNode()
    {
        NodeType = NodeType.Default;
        IsActive = true;
        ConnectedNodes = new List<int>();
    }

    public MapNode(NodeType nodeType)
    {
        NodeType = nodeType;
        IsActive = true;
        ConnectedNodes = new List<int>();
    }
}
