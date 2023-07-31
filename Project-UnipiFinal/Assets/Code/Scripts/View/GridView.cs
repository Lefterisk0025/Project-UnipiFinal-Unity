using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridView : MonoBehaviour, IObserver
{
    GridPresenter _gridPresenter;
    List<Subject> _tileSubjects;

    TileView _tile1Clicked;
    TileView _tile2Clicked;

    [SerializeField] private List<TileView> _tilesPrefabs;
    [SerializeField] private int _height;
    [SerializeField] private TextMeshProUGUI actionsText;

    private void Awake()
    {
        _gridPresenter = new GridPresenter(this);

        _tileSubjects = new List<Subject>();
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
            // Calculate index based on tiles value
            int tilePrefabIndex = tile.Value - 1;

            var spawnedTile = Instantiate(_tilesPrefabs[tilePrefabIndex], transform);

            spawnedTile.GetComponent<TileView>().Tile = tile;

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
            case Actions.SelectTiles:
                var tile = (TileView)subject;
                SelectTile(tile);
                break;
            default:
                break;
        }
    }

    private void SelectTile(TileView tileView)
    {
        if (_tile1Clicked == null)
        {
            _tile1Clicked = tileView;
            _tile1Clicked.UpdateView(TileState.Selected);
            return;
        }

        if (_tile2Clicked == null)
        {
            // Check if the same tile is being selected twice
            if (_tile1Clicked == tileView)
                return;

            _tile2Clicked = tileView;
        }

        _gridPresenter.CheckForMatch(_tile1Clicked, _tile2Clicked);

        _tile1Clicked = null;
        _tile2Clicked = null;
    }
}
