using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public int Id { get; set; }
    public NodeType NodeType { get; set; }
    public bool IsActive { get; set; }
    public Objective Objective { get; set; }

    public MapNode()
    {
        NodeType = NodeType.Default;
        IsActive = true;
        Objective = new Objective();
    }

    public MapNode(NodeType nodeType)
    {
        NodeType = nodeType;
        IsActive = true;
        Objective = new Objective();
    }
}
