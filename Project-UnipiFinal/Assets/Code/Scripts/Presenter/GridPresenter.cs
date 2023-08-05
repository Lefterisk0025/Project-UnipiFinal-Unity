using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPresenter
{
    GridView _gridView;
    Grid _grid;
    TilesComparator _tilesComparator;

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

    public List<Tile> CreateTiles(int height)
    {
        _grid = new Grid(height);

        GenerateRandomSeed();

        Tile tile;
        for (int row = 0; row < _grid.Height; row++)
        {
            for (int col = 0; col < _grid.Width; col++)
            {
                //int value = Random.Range(1, 10);
                int value = 7;
                Vector2 positionInGrid = new Vector2((float)row, (float)col);

                tile = new Tile(value, positionInGrid);

                _grid.AddTile(tile);
            }
        }

        return _grid.Tiles;
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

            float tile1Row = tile1View.Tile.PositionInGrid.x;
            float tile2Row = tile2View.Tile.PositionInGrid.x;

            if (CanRemoveTilesInRow(tile1Row))
            {
                List<Tile> tilesToBeRemoved1 = RemoveTilesInRow(tile1Row);
                _gridView.RemoveTiles(tilesToBeRemoved1);
            }

            if (CanRemoveTilesInRow(tile2Row))
            {
                List<Tile> tilesToBeRemoved2 = RemoveTilesInRow(tile2Row);
                _gridView.RemoveTiles(tilesToBeRemoved2);
            }
        }
        else
        {
            tile1View.UpdateView(TileState.Default);
            tile2View.UpdateView(TileState.Default);
        }
    }

    public List<Tile> CreateGridLineBasedOnActiveTiles()
    {
        List<Tile> tilesLine = new List<Tile>();

        // Get the remaining active tiles
        var activeTilesList = _grid.GetActiveTiles();
        Debug.Log("Active tiles number" + activeTilesList.Count);
        Debug.Log("Active tiles number" + activeTilesList.Count);

        // Calculate the height of the newly added part of the grid 
        int subGridHeight = (int)Mathf.Ceil((float)activeTilesList.Count / (float)_grid.Width);

        Tile tile;

        // Calculate initial row and col values based on last tiles position
        float row = _grid.Tiles[^1].PositionInGrid.x;
        float col = _grid.Tiles[^1].PositionInGrid.y + 1;
        if (col > _grid.Width - 1)
        {
            row = _grid.Height;
            col = 0;
        }

        foreach (Tile activeTile in activeTilesList)
        {
            Vector2 positionInGrid = new Vector2(row, col);

            tile = new Tile(activeTile.Value, positionInGrid);

            _grid.AddTile(tile);
            tilesLine.Add(tile);

            col++;
            if (col > _grid.Width - 1)
            {
                row++;
                col = 0;
            }
        }

        _grid.Height = _grid.Height + subGridHeight;

        Debug.Log("Add tiles line");

        return tilesLine;
    }

    private bool CanRemoveTilesInRow(float row)
    {
        // FIX ROWS WHERE THERE ARE NOT ALL THE COLS

        Tile tile;
        for (float y = 0; y < _grid.Width; y++)
        {
            tile = _grid.GetTileInPosition(new Vector2(row, y));
            if (tile.IsActive)
                return false;
        }

        return true;
    }

    private List<Tile> RemoveTilesInRow(float row)
    {
        Debug.Log("Size before deletion: " + _grid.Tiles.Count);
        Tile tile;
        List<Tile> tilesToBeRemovedFromScene = new List<Tile>();
        for (float y = 0; y < _grid.Width; y++)
        {
            tile = _grid.GetTileInPosition(new Vector2(row, y));

            tilesToBeRemovedFromScene.Add(tile);
            _grid.RemoveTile(tile);
        }

        Debug.Log("Remove tiles at row " + row);

        Debug.Log("Size after deletion: " + _grid.Tiles.Count);
        _grid.Height--;

        // Update remaing tiles position
        float x1 = 0;
        float y1 = 0;
        foreach (Tile tile1 in _grid.Tiles)
        {

            tile1.PositionInGrid = new Vector2(x1, y1);

            y1++;
            if (y1 > _grid.Width - 1)
            {
                x1++;
                y1 = 0;
            }
        }

        return tilesToBeRemovedFromScene;
    }

    public void PrintTiles()
    {
        Debug.Log("-------------- START TILES --------------");
        foreach (Tile tile in _grid.Tiles)
        {
            Debug.Log(tile.PositionInGrid);
        }
        Debug.Log("-------------- END TILES --------------");
    }
}
