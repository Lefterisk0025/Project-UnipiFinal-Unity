using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchResults
{
    public int TotalScore { get; set; }
    public int MatchesFound { get; set; }
    public int ReputationEarned { get; set; }
    public bool IsVictory { get; set; }
}
