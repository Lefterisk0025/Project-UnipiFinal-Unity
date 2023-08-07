using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MatchDataSO : ScriptableObject
{
    public enum GameMode { MATCH_POINT, TIME_ATTACK }

    public GameMode gameMode;
}
