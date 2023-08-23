using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    private NodeType _nodeType;
    private bool _isActive;

    public NodeType NodeType { get => _nodeType; set => _nodeType = value; }
    public bool IsActive { get => _isActive; set => _isActive = value; }

    public MapNode(NodeType nodeType)
    {
        _nodeType = nodeType;
        _isActive = true;
    }
}
