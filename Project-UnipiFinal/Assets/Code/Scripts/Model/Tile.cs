using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private bool _isActive;

    public int Value { get; set; }
    public Vector2 PositionInGrid { get; set; }
    public bool IsActive { get => _isActive; set => _isActive = value; }

    public Tile(int value, Vector2 positionInGrid)
    {
        Value = value;
        PositionInGrid = positionInGrid;
        IsActive = true;
    }
    public void ActivateTile() => _isActive = true;
    public void DeactivateTile() => _isActive = false;

}
