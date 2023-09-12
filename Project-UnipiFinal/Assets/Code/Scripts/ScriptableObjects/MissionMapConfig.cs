using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class MissionMapConfig : ScriptableObject
{
    [Header("General Settings")]
    public Difficulty Difficulty;
    public Vector2Int MapDepth;
    public int MaxNodesPerVerticalLine;

    [Header("Time Attack Game Mode")]
    public int TimeToFindNumberOfMatchesInSec;
    public int NumberOfMatches;

    public int ScoreGoal;
    public int PointsPerMatch;
}
