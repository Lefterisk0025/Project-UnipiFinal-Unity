using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class MissionMapConfig : ScriptableObject
{
    public Difficulty Difficulty;
    public Vector2Int MapDepth;
    public int MaxNodesPerLine;
}
