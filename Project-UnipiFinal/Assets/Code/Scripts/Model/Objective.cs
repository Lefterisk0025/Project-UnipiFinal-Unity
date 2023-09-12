using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective
{
    public int Id { get; set; }
    public Grid Grid { get; set; }
    public int MapNodeId { get; set; }
    public string Difficulty { get; set; }
    public GameMode GameMode { get; set; }
}
