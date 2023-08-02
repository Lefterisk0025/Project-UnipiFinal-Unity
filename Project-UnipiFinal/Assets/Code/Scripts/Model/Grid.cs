using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private const int width = 9;

    public int Width => width;
    public int Height { get; set; }
    public List<Tile> Tiles { get; set; }

    public Grid(int height)
    {
        Height = height;

        Tiles = new List<Tile>();
    }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    public void RemoveTile(Tile tile)
    {
        Tiles.Remove(tile);
    }

    public Tile GetTileInPosition(Vector2 pos)
    {
        foreach (var tile in Tiles)
        {
            if (pos == tile.PositionInGrid)
                return tile;
        }

        return null;
    }

    public List<Tile> GetActiveTiles()
    {
        var tempTileList = new List<Tile>();

        foreach (var tile in Tiles)
        {
            if (tile.IsActive)
                tempTileList.Add(tile);
        }

        return tempTileList;
    }
}
