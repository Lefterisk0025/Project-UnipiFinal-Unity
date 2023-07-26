using System.Collections;
using System.Collections.Generic;

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
}
