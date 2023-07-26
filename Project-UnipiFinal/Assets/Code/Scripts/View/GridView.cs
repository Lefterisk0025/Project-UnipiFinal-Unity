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
                if (_tile1Clicked == null)
                {
                    _tile1Clicked = tile;
                    _tile1Clicked.UpdateView(TileState.Selected);
                    return;
                }

                if (_tile2Clicked == null)
                {
                    _tile2Clicked = tile;
                    CheckForMatch(_tile1Clicked, _tile2Clicked);
                }
                break;
            default:
                // code block
                break;
        }

    }

    private void CheckForMatch(TileView tile1, TileView tile2)
    {
        bool matchFound = _gridPresenter.IsMatchingTile(tile1.Tile, tile2.Tile);
        if (matchFound)
        {
            _gridPresenter.DeactivateTile(tile1);
            _gridPresenter.DeactivateTile(tile2);

            actionsText.text = "Found match!";
        }
        else
        {
            _tile1Clicked.UpdateView(TileState.Default);
            _tile2Clicked.UpdateView(TileState.Default);
            actionsText.text = "Didn't find a match!";
        }

        _tile1Clicked = null;
        _tile2Clicked = null;
    }
}
