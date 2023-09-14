using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchConfig : ScriptableObject
{
    public Difficulty Difficulty;
    public int TotalTime;
    public int Height;
}
