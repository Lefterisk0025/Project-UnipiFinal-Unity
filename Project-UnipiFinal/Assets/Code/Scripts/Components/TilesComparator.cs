using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesComparator
{
    enum SearchDirections { Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft, LeftPath }
    SearchDirections searchDirections;

    Grid _grid;

    public TilesComparator(Grid grid)
    {
        _grid = grid;
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
        // Check positions
        if (dir.dx == 0)
        {
            if (dir.dy > 0) // Right
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Right);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else // Left
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Left);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
        }
        else if (dir.dy == 0)
        {
            if (dir.dx > 0) // Bottom
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Bottom);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else // Top
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.Top);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
        }

        bool areDiagonal = AreDiagonallyAligned(originTile.PositionInGrid, targetTile.PositionInGrid);
        // If tiles are not diagonally aligned, check the tiles following the left path the origin tile to the target tile
        if (!areDiagonal)
        {
            // Check if tagret tile is below origin tile
            if (originTile.PositionInGrid.x <= targetTile.PositionInGrid.x)
                return false;

            // Check if target tile is to the above right
            if ((originTile.PositionInGrid.x == originTile.PositionInGrid.x + 1) && (targetTile.PositionInGrid.y == 8))
            {
                return true;
            }

            haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.LeftPath);
            if (haveTilesInBetween)
                return false;
            else
                return true;
        }

        if (dir.dx > 0) // Bottom
        {
            if (dir.dy > 0) // Right
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.BottomRight);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else if (dir.dy < 0) // Left
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.BottomLeft);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
        }

        if (dir.dx < 0) // Top
        {
            if (dir.dy > 0) // Right
            {
                haveTilesInBetween = HaveTilesInBetween(originTile.PositionInGrid, targetTile.PositionInGrid, SearchDirections.TopRight);
                if (haveTilesInBetween)
                    return false;
                else
                    return true;
            }
            else if (dir.dy < 0) // Left
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
                    Tile tile = _grid.GetTileInPosition(new Vector2(originTilePos.x, y));
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
                    Tile tile = _grid.GetTileInPosition(new Vector2(originTilePos.x, y));
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
                    Tile tile = _grid.GetTileInPosition(new Vector2(x, originTilePos.y));
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
                    Tile tile = _grid.GetTileInPosition(new Vector2(x, originTilePos.y));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                }
                break;
            case SearchDirections.TopLeft:
                float y2 = originTilePos.y - 1;
                for (float x = originTilePos.x - 1; x > targetTilePos.x; x--)
                {
                    Tile tile = _grid.GetTileInPosition(new Vector2(x, y2));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                    y2--;
                }
                break;
            case SearchDirections.TopRight:
                float y3 = originTilePos.y + 1;
                for (float x = originTilePos.x - 1; x > targetTilePos.x; x--)
                {
                    Tile tile = _grid.GetTileInPosition(new Vector2(x, y3));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                    y3++;
                }
                break;
            case SearchDirections.BottomLeft:
                float y4 = originTilePos.y - 1;
                for (float x = originTilePos.x + 1; x < targetTilePos.x; x++)
                {
                    Tile tile = _grid.GetTileInPosition(new Vector2(x, y4));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                    y4--;
                }
                break;
            case SearchDirections.BottomRight:
                float y5 = originTilePos.y + 1;
                for (float x = originTilePos.x + 1; x < targetTilePos.x; x++)
                {
                    Tile tile = _grid.GetTileInPosition(new Vector2(x, y5));
                    if (tile.IsActive)
                    {
                        foundTiles = true;
                        break;
                    }
                    y5++;
                }
                break;
            case SearchDirections.LeftPath:

                float tempStartY = originTilePos.y - 1;
                float tempEndY = 0;

                for (float x = originTilePos.x; x >= targetTilePos.x; x--)
                {
                    if (x == targetTilePos.x)
                        tempEndY = targetTilePos.y;

                    for (float y = tempStartY; y > tempEndY; y--)
                    {
                        Tile tile = _grid.GetTileInPosition(new Vector2(x, y));
                        if (tile.IsActive)
                        {
                            foundTiles = true;
                            break;
                        }
                    }

                    tempStartY = _grid.Width - 1;
                }
                break;
        }

        if (foundTiles)
            return true;
        else
            return false;
    }
}
