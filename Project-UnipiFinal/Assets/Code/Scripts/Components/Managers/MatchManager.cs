using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    [SerializeField] private GridView _gridView;

    MissionService _missionService;
    MapNode _currMapNode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _missionService = new MissionService();
    }

    private async void Start()
    {
        // Load list with the connected nodes
        List<MapNode> connectedMapNodesList = await _missionService.LoadLocalMapNodesList();
        int selectedMapNodeId = PlayerPrefs.GetInt("SelectedNodeId");

        // Get selected node from list
        int nodeIndex = 0;
        for (int i = 0; i < connectedMapNodesList.Count; i++)
        {
            if (connectedMapNodesList[i].Id == selectedMapNodeId)
            {
                _currMapNode = connectedMapNodesList[i];
                nodeIndex = i;
                break;
            }
        }

        // Load or create new grid
        Grid newGrid = _currMapNode.Objective.Grid;
        if (newGrid == null)
        {
            newGrid = _gridView.InitializeGrid(8);

            _gridView.GenerateTilesOnScene(newGrid.Tiles);

            connectedMapNodesList[nodeIndex].Objective.Grid = newGrid;

            if (await _missionService.SaveLocalMapNodesListData(connectedMapNodesList))
                Debug.Log("Save success!");
            else
                Debug.Log("Save failed...");
        }
        else
        {
            _gridView.GenerateTilesOnScene(newGrid.Tiles);
        }
    }

}
