using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPerformance
{
    public bool IsVictory { get; set; }
    public int TotalMissionScore { get; set; }
    public int TotalReputation { get; set; }

    public MissionPerformance()
    {
        IsVictory = false;
        TotalMissionScore = 0;
        TotalReputation = 0;
    }
}
