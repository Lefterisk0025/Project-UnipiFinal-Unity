using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public NodeType NodeType { get; set; }
    public bool IsActive { get; set; }

    public MapNode()
    {
        NodeType = NodeType.Default;
        IsActive = true;
    }

    public MapNode(NodeType nodeType)
    {
        NodeType = nodeType;
        IsActive = true;
    }
}
