using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public Grid Grid { get; set; }
    public int NodeId { get; set; }
    public string Difficulty { get; set; }
    public GameMode GameMode { get; set; }
}
