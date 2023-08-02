using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPresenter
{
    GridView _gridView;
    Grid _grid;
    TilesComparator _tilesComparator;
    List<Tile> _lastAddedTilesLine;

    public GridPresenter(GridView gridView)
    {
        _gridView = gridView;
    }

    public int GetGridWidth()
    {
        return _grid.Width;
    }

    public int GetGridHeight()
    {
        return _grid.Height;
    }

    public void RemoveTileFromGrid(Tile tile)
    {
        _grid.RemoveTile(tile);
    }

    public void DecreaseGridHeightByNumber(int num)
    {
        _grid.Height -= num;
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

    public void DeactivateTile(TileView tileView)
    {
        tileView.UpdateView(TileState.Deactivated);
        tileView.Tile.DeactivateTile();
    }

    public void ValidateTileMatch(TileView tile1View, TileView tile2View)
    {
        if (_grid == null)
            return;

        _tilesComparator = new TilesComparator(_grid);

        bool matchFound = _tilesComparator.IsMatchingTile(tile1View.Tile, tile2View.Tile);

        if (matchFound)
        {
            DeactivateTile(tile1View);
            DeactivateTile(tile2View);

            if (CanRemoveLineInRow(tile1View.Tile.PositionInGrid.x))
            {
                _gridView.RemoveTilesLine(tile1View.Tile.PositionInGrid.x);
                UpdateTilesPosition();
            }

            if (CanRemoveLineInRow(tile2View.Tile.PositionInGrid.x))
            {
                _gridView.RemoveTilesLine(tile2View.Tile.PositionInGrid.x);
                UpdateTilesPosition();
            }
        }
        else
        {
            tile1View.UpdateView(TileState.Default);
            tile2View.UpdateView(TileState.Default);
        }
    }

    public List<Tile> CreateGridLine()
    {
        _lastAddedTilesLine = new List<Tile>();

        // Get the remaining active tiles
        var activeTilesList = _grid.GetActiveTiles();

        // Calculate the height of the newly added part of the grid 
        int subGridHeight = (int)Mathf.Ceil((float)activeTilesList.Count / (float)_grid.Width);

        int newGridHeight = _grid.Height + subGridHeight;

        Tile tile;
        int i = 0; // Tiles counter
        for (float row = _grid.Height; row < newGridHeight; row++)
        {
            for (float col = 0; col < _grid.Width; col++)
            {
                Vector2 positionInGrid = new Vector2(row, col);

                tile = new Tile(activeTilesList[i].Value, positionInGrid);

                _grid.AddTile(tile);
                _lastAddedTilesLine.Add(tile);

                i++;
                if (i > activeTilesList.Count - 1) break;
            }
        }

        _grid.Height = newGridHeight;

        return _lastAddedTilesLine;
    }

    private bool CanRemoveLineInRow(float row)
    {
        Tile tempTile;
        for (float y = 0; y < _grid.Width; y++)
        {
            tempTile = _grid.GetTileInPosition(new Vector2(row, y));
            if (tempTile.IsActive)
                return false;
        }

        return true;
    }

    private void UpdateTilesPosition()
    {
        float col = 0;
        float row = 0;
        foreach (Tile tile in _grid.Tiles)
        {
            tile.PositionInGrid = new Vector2(row, col);

            col++;
            if (col > _grid.Width - 1)
            {
                row++;
                col = 0;
            }

        }
    }

    private void PrintTiles()
    {
        foreach (Tile tile in _grid.Tiles)
        {
            Debug.Log(tile.PositionInGrid);
        }
    }
}
