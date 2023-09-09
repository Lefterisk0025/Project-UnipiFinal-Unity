using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int Value { get; set; }
    public Vector2 PositionInGrid { get; set; }
    public bool IsActive { get; set; }

    public Tile(int value, Vector2 positionInGrid)
    {
        Value = value;
        PositionInGrid = positionInGrid;
        IsActive = true;
    }
    public override string ToString()
    {
        return "Tile " + "(" + PositionInGrid.x + ", " + PositionInGrid.y + ") " + "value: " + Value;
    }

}
