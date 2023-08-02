using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridView : MonoBehaviour, IObserver
{

    GridPresenter _gridPresenter;
    List<Subject> _tileSubjects;

    TileView _selectedTile1;
    TileView _selectedTile2;

    [SerializeField] private List<TileView> _tilesPrefabs;
    [SerializeField] private List<GameObject> _tilesGameObjects;
    [SerializeField] private int _height;
    [SerializeField] private TextMeshProUGUI actionsText;

    private void Awake()
    {
        _gridPresenter = new GridPresenter(this);

        _tileSubjects = new List<Subject>();

        _tilesGameObjects = new List<GameObject>();
    }

    private void OnDisable()
    {
        //_tileSubject.RemoveObserver(this);
    }

    private void Start()
    {
        _gridPresenter.GenerateGrid(_height);

        actionsText.text = "";
    }

    public void SetGrid(Grid grid)
    {
        foreach (var tile in grid.Tiles)
        {
            int tilePrefabIndex = tile.Value - 1;

            var spawnedTile = Instantiate(_tilesPrefabs[tilePrefabIndex], transform);

            spawnedTile.GetComponent<TileView>().Tile = tile;

            _tilesGameObjects.Add(spawnedTile.gameObject);

            // Create list of subjects to observe
            _tileSubjects.Add(spawnedTile);
        }

        // Become observer of each tile
        foreach (var subject in _tileSubjects)
        {
            subject.AddObserver(this);
        }
    }

    public void OnNotify(ISubject subject, Actions action)
    {
        switch (action)
        {
            case Actions.SelectTile:
                var tile = (TileView)subject;
                OnTileSelected(tile);
                break;
            default:
                break;
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

        _gridPresenter.ValidateTileMatch(_selectedTile1, _selectedTile2);

        _selectedTile1 = null;
        _selectedTile2 = null;
    }

    public void AddLineToGrid()
    {
        var lastAddedLine = _gridPresenter.CreateGridLine();
        if (lastAddedLine == null)
            return;

        int _tileSubjectsPrevListSize = _tileSubjects.Count; // helper var for updating the newly added subjects

        foreach (Tile tile in lastAddedLine)
        {
            int tilePrefabIndex = tile.Value - 1;

            var spawnedTile = Instantiate(_tilesPrefabs[tilePrefabIndex], transform);

            spawnedTile.GetComponent<TileView>().Tile = tile;

            _tilesGameObjects.Add(spawnedTile.gameObject);

            _tileSubjects.Add(spawnedTile);
        }

        // Become observer of each NEW tile starting from the index that the new subjectss added
        for (int i = _tileSubjectsPrevListSize; i < _tileSubjects.Count; i++)
        {
            _tileSubjects[i].AddObserver(this);
        }
    }

    public void RemoveTilesLine(float row)
    {
        foreach (GameObject tileGO in _tilesGameObjects)
        {
            var tempTileView = tileGO.GetComponent<TileView>();
            if (tempTileView.Tile.PositionInGrid.x == row)
            {
                _gridPresenter.RemoveTileFromGrid(tempTileView.Tile);
                tileGO.SetActive(false);
            }
        }

        _gridPresenter.DecreaseGridHeightByNumber(1);
    }
}
