using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPresenter
{
    GridView _gridView;
    Grid _grid;
    TilesComparator _tilesComparator;
    MissionService _missionService;

    public GridPresenter(GridView gridView)
    {
        _gridView = gridView;
        _missionService = new MissionService();
    }

    public Grid CreateAndInitializeGrid(int height)
    {
        _grid = new Grid(height);
        _grid.Tiles = CreateTiles(height);

        _gridView.GenerateTilesOnScene(_grid.Tiles);

        return _grid;
    }

    public void InitializeGrid(Grid grid)
    {
        _grid = grid;

        _gridView.GenerateTilesOnScene(_grid.Tiles);
    }

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
    }

    public List<Tile> CreateTiles(int height)
    {
        List<Tile> tempTiles = new List<Tile>();

        GenerateRandomSeed();

        Tile tile;
        for (int row = 0; row < _grid.Height; row++)
        {
            for (int col = 0; col < _grid.Width; col++)
            {
                int value = Random.Range(1, 10);
                //int value = 7;
                Vector2 positionInGrid = new Vector2((float)row, (float)col);

                tile = new Tile(value, positionInGrid);

                tempTiles.Add(tile);
            }
        }

        return tempTiles;
    }

    public void DeactivateTile(TileView tileView)
    {
        tileView.UpdateView(TileState.Deactivated);
        tileView.Tile.IsActive = false;
    }

    public bool ValidateTileMatch(TileView tile1View, TileView tile2View)
    {
        if (_grid == null)
        {
            Debug.Log("There is no grid bro...");
            return false;
        }


        _tilesComparator = new TilesComparator(_grid);

        bool matchFound = _tilesComparator.IsMatchingTile(tile1View.Tile, tile2View.Tile);

        if (matchFound)
        {
            DeactivateTile(tile1View);
            DeactivateTile(tile2View);

            float tile1Row = tile1View.Tile.PositionInGrid.x;
            float tile2Row = tile2View.Tile.PositionInGrid.x;

            if (CanRemoveTilesLineInRow(tile1Row))
            {
                List<Tile> tilesToBeRemoved1 = RemoveTilesInRow(tile1Row);
                _gridView.RemoveTiles(tilesToBeRemoved1);
            }

            if (CanRemoveTilesLineInRow(tile2Row))
            {
                List<Tile> tilesToBeRemoved2 = RemoveTilesInRow(tile2Row);
                _gridView.RemoveTiles(tilesToBeRemoved2);
            }

            return true;
        }
        else
        {
            tile1View.UpdateView(TileState.Default);
            tile2View.UpdateView(TileState.Default);

            return false;
        }
    }

    public List<Tile> CreateGridLineBasedOnActiveTiles()
    {
        List<Tile> tilesLine = new List<Tile>();

        // Get the remaining active tiles
        var activeTilesList = _grid.GetActiveTiles();

        // Calculate the height of the newly added part of the grid 
        int subGridHeight = (int)Mathf.Ceil((float)activeTilesList.Count / (float)_grid.Width);

        // Calculate initial row and col values based on last tile's position
        float row = _grid.Tiles[^1].PositionInGrid.x;
        float col = _grid.Tiles[^1].PositionInGrid.y + 1;
        if (col > _grid.Width - 1)
        {
            row = _grid.Height;
            col = 0;
        }

        // Create new tiles based on the active tiles
        Tile tile;
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

        Debug.Log("Add line");

        return tilesLine;
    }

    private bool CanRemoveTilesLineInRow(float row)
    {
        float tempLastY = _grid.Tiles[^1].PositionInGrid.y;

        Tile tile;
        for (float y = 0; y <= tempLastY; y++)
        {
            tile = _grid.GetTileInPosition(new Vector2(row, y));

            if (tile == null || tile.IsActive)
                return false;
        }

        return true;
    }

    private List<Tile> RemoveTilesInRow(float row)
    {
        float tempLastY = _grid.Tiles[^1].PositionInGrid.y;

        Tile tile;
        List<Tile> tilesToBeRemovedFromScene = new List<Tile>();
        for (float y = 0; y <= tempLastY; y++)
        {
            tile = _grid.GetTileInPosition(new Vector2(row, y));
            if (tile == null)
                break;

            tilesToBeRemovedFromScene.Add(tile);
            _grid.RemoveTile(tile);
        }

        Debug.Log("Remove tiles at row " + row);

        _grid.Height--;

        ReInitializeTilesPosition();

        return tilesToBeRemovedFromScene;
    }

    private void ReInitializeTilesPosition()
    {
        float x = 0;
        float y = 0;
        foreach (Tile tile in _grid.Tiles)
        {
            tile.PositionInGrid = new Vector2(x, y);

            y++;
            if (y > _grid.Width - 1)
            {
                x++;
                y = 0;
            }
        }
    }

    public void PrintTiles()
    {
        Debug.Log("-------------- START TILES --------------");
        foreach (Tile tile in _grid.Tiles)
        {
            Debug.Log(tile.ToString());
        }
        Debug.Log("-------------- END TILES --------------");
    }
}
