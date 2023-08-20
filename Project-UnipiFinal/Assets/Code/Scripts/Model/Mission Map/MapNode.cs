using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    private NodeType _nodeType;

    public NodeType NodeType { get => _nodeType; set => _nodeType = value; }

    public MapNode(NodeType nodeType)
    {
        _nodeType = nodeType;
    }
}
