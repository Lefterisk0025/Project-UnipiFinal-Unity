using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GridView : MonoBehaviour, IObserver
{
    GridPresenter _gridPresenter;

    TileView _selectedTile1;
    TileView _selectedTile2;

    [SerializeField] private List<TileView> _tilesPrefabs;
    private IDictionary<Tile, TileView> _spawnedTiles;
    [SerializeField] private int _height;

    public UnityEvent OnMatchFound;

    private void Awake()
    {
        //_gridPresenter = new GridPresenter(this);
    }

    private void OnDisable()
    {
        _spawnedTiles.Clear();
    }

    private void OnEnable()
    {
        _spawnedTiles = new Dictionary<Tile, TileView>();
    }

    public void InjectGridPresenter(GridPresenter gridPresenter)
    {
        _gridPresenter = gridPresenter;
    }

    public void OnNotify(ISubject subject, Actions action)
    {
        switch (action)
        {
            case Actions.SELECT_TILE:
                var tile = (TileView)subject;
                OnTileSelected(tile);
                break;
            default:
                break;
        }
    }

    // public Grid InitializeGrid(int height)
    // {
    //     return _gridPresenter.InitializeGrid(height);
    // }

    public void GenerateTilesOnScene(List<Tile> tiles)
    {
        TileView spawnedTileView;
        foreach (var tile in tiles)
        {
            int tilePrefabIndex = tile.Value - 1;

            var spawnedTile = Instantiate(_tilesPrefabs[tilePrefabIndex], transform);

            spawnedTileView = spawnedTile.GetComponent<TileView>();
            spawnedTileView.Tile = tile;

            spawnedTileView.AddObserver(this);

            _spawnedTiles[tile] = spawnedTile;
        }
    }

    private void OnTileSelected(TileView tileView)
    {
        if (_selectedTile1 == null)
        {
            _selectedTile1 = tileView;
            _selectedTile1.UpdateView(TileState.Selected);
            return;
        }

        if (_selectedTile2 == null)
        {
            // Check if the same tile is being selected twice
            if (_selectedTile1 == tileView)
                return;

            _selectedTile2 = tileView;
        }

        if (_gridPresenter.ValidateTileMatch(_selectedTile1, _selectedTile2))
            OnMatchFound.Invoke();

        _selectedTile1 = null;
        _selectedTile2 = null;
    }

    public void AddLineToGrid()
    {
        List<Tile> tilesLine = _gridPresenter.CreateGridLineBasedOnActiveTiles();
        if (tilesLine == null)
            return;

        GenerateTilesOnScene(tilesLine);
    }

    public void RemoveTiles(List<Tile> tiles)
    {
        if (tiles == null)
            return;

        foreach (Tile tile in tiles)
        {
            if (tile == null)
                break;

            _spawnedTiles[tile].gameObject.SetActive(false);
        }
    }

    public void PrintTiles()
    {
        _gridPresenter.PrintTiles();
    }

    public void ClearGrid()
    {
        _gridPresenter.ClearGrid();

        _spawnedTiles.Clear();

        foreach (Transform child in this.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
