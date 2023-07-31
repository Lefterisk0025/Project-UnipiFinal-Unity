using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridPresenter
{
    enum SearchDirections { Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft }
    SearchDirections searchDirections;

    GridView _gridView;
    Grid _grid;

    public GridPresenter(GridView gridView)
    {
        _gridView = gridView;
    }

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
    }

    public void GenerateGrid(int height)
    {
        _grid = new Grid(height);

        GenerateRandomSeed();

        Tile tile;
        for (int row = 0; row < _grid.Height; row++)
        {
            for (int col = 0; col < _grid.Width; col++)
            {
                int value = Random.Range(1, 10);
                Vector2 positionInGrid = new Vector2((int)row, (int)col);

                tile = new Tile(value, positionInGrid);

                _grid.AddTile(tile);
            }
        }

        _gridView.SetGrid(_grid);
    }

    public Tile GetTileInPosition(Vector2 pos)
    {
        foreach (var tile in _grid.Tiles)
        {
            if (pos == tile.PositionInGrid)
                return tile;
        }

        return null;
    }

    public void DeactivateTile(TileView tileView)
    {
        tileView.UpdateView(TileState.Deactivated);
        tileView.Tile.DeactivateTile();
    }

    public void CheckForMatch(TileView tile1, TileView tile2)
    {
        bool matchFound = IsMatchingTile(tile1.Tile, tile2.Tile);
        if (matchFound)
        {
            DeactivateTile(tile1);
            DeactivateTile(tile2);
        }
        else
        {
            tile1.UpdateView(TileState.Default);
            tile2.UpdateView(TileState.Default);
        }
    }

    public bool IsMatchingTile(Tile originTile, Tile targetTile)
    {
        bool areTilesValid = AreTilesInValidPositionForMatch(originTile, targetTile);
        if (areTilesValid)
        {
            if (originTile.Value == targetTile.Value || originTile.Value + targetTile.Value == 10)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    private bool AreTilesInValidPositionForMatch(Tile originTile, Tile targetTile)
    {
        bool haveTilesInBetween = false;
        var dir = DirectionTo(originTile.PositionInGrid, targetTile.PositionInGrid);
        // Check top, bottom, left, right and diagonal positions
        if (dir.dx == 0) // Right - Left
        {
            if (dir.dy > 0)
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Right);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else
            {
                Debug.Log("Target tile is to the left of the base tile.");
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Left);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
        }
        else if (dir.dy == 0) // Bottom - Top
        {
            if (dir.dx > 0)
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Bottom);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Top);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
        }

        bool areDiagonal = AreDiagonallyAligned(originTile.PositionInGrid, targetTile.PositionInGrid);
        if (!areDiagonal)
        {
            Debug.Log("Base tile: " + new Vector2(originTile.PositionInGrid.x, originTile.PositionInGrid.y));
            Debug.Log("Target tile: " + new Vector2(targetTile.PositionInGrid.x, targetTile.PositionInGrid.y));

            if (originTile.PositionInGrid.x <= targetTile.PositionInGrid.x)
                return false;

            var tempY = originTile.PositionInGrid.y - 1;
            for (float x = originTile.PositionInGrid.x; x > targetTile.PositionInGrid.x; x--)
            {
                for (float y = tempY; y >= targetTile.PositionInGrid.y; y--)
                {
                    Debug.Log(new Vector2(x, y));
                }

                tempY = _grid.Width - 1;
            }
        }

        if (dir.dx > 0)
        {
            if (dir.dy > 0)
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.BottomRight);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else if (dir.dy < 0)
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.BottomLeft);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
        }

        if (dir.dx < 0)
        {
            if (dir.dy > 0)
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.TopRight);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else if (dir.dy < 0)
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.TopLeft);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }

            return true;
        }

        return false;
    }

    private (int dx, int dy) DirectionTo(Vector2 originTilePos, Vector2 targetTilePos)
    {
        return ((int)(targetTilePos.x - originTilePos.x), (int)(targetTilePos.y - originTilePos.y));
    }

    private bool AreDiagonallyAligned(Vector2 originTilePos, Vector2 targetTilePos)
    {
        return (Mathf.Abs((originTilePos.x - targetTilePos.x)) == Mathf.Abs((originTilePos.y - targetTilePos.y)));
    }

    private bool HaveTilesInBetween(Vector2 originTilePos, Vector2 targetTilePos, SearchDirections directionTo)
    {
        // Check if tiles are neighboring, so there is no need for run
        var dist = Vector2.Distance(originTilePos, targetTilePos);
        if (dist == 1f || dist == 1.4142135623731f)
            return false;

        bool foundTiles = false;
        switch (directionTo)
        {
            case SearchDirections.Right:
                for (float y = originTilePos.y + 1; y < targetTilePos.y; y++)
                {
                    Tile tile = GetTileInPosition(new Vector2(originTilePos.x, y));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                }
                break;
            case SearchDirections.Left:
                for (float y = originTilePos.y - 1; y > targetTilePos.y; y--)
                {
                    Tile tile = GetTileInPosition(new Vector2(originTilePos.x, y));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                }
                break;
            case SearchDirections.Top:
                for (float x = originTilePos.x - 1; x > targetTilePos.x; x--)
                {
                    Tile tile = GetTileInPosition(new Vector2(x, originTilePos.y));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                }
                break;
            case SearchDirections.Bottom:
                for (float x = originTilePos.x + 1; x < targetTilePos.x; x++)
                {
                    Tile tile = GetTileInPosition(new Vector2(x, originTilePos.y));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                }
                break;
            case SearchDirections.TopLeft:
                for (float x = originTilePos.x - 1; x > targetTilePos.x; x--)
                {
                    for (float y = originTilePos.y - 1; y > targetTilePos.y; y--)
                    {
                        Tile tile = GetTileInPosition(new Vector2(x, y));
                        if (tile.IsActive)
                        {
                            foundTiles = true;
                            break;
                        }
                    }
                }
                break;
            case SearchDirections.TopRight:
                for (float x = originTilePos.x - 1; x > targetTilePos.x; x--)
                {
                    for (float y = originTilePos.y + 1; y < targetTilePos.y; y++)
                    {
                        Tile tile = GetTileInPosition(new Vector2(x, y));
                        if (tile.IsActive)
                        {
                            foundTiles = true;
                            break;
                        }
                    }
                }
                break;
            case SearchDirections.BottomLeft:
                for (float x = originTilePos.x + 1; x < targetTilePos.x; x++)
                {
                    for (float y = originTilePos.y - 1; y > targetTilePos.y; y--)
                    {
                        Tile tile = GetTileInPosition(new Vector2(x, y));
                        if (tile.IsActive)
                        {
                            foundTiles = true;
                            break;
                        }
                    }
                }
                break;
            case SearchDirections.BottomRight:
                for (float x = originTilePos.x + 1; x < targetTilePos.x; x++)
                {
                    for (float y = originTilePos.y + 1; y < targetTilePos.y; y++)
                    {
                        Tile tile = GetTileInPosition(new Vector2(x, y));
                        if (tile.IsActive)
                        {
                            foundTiles = true;
                            break;
                        }
                    }
                }
                break;
        }

        if (foundTiles)
            return true;
        else
            return false;
    }
}
