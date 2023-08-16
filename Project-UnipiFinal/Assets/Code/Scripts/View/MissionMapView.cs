using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Build.Pipeline.Tasks;

public class MissionMapView : MonoBehaviour
{
    MissionMapPresenter _missionMapPresenter;

    [SerializeField] private TextMeshProUGUI _missionTitleGUI;
    [SerializeField] private TextMeshProUGUI _missionDifficultyGUI;
    [SerializeField] private UILineRenderer _uiLineRenderer;

    [SerializeField] private MapNode _attackNodePrefab;
    [SerializeField] private MapNode _boostHubNodePrefab;

    [SerializeField] private int mapNodesCount;
    [SerializeField] private int mapDepth;

    private void Awake()
    {
        _missionMapPresenter = new MissionMapPresenter(this);
    }

    private void OnEnable()
    {
        Mission mission = _missionMapPresenter.GetLocalSavedMission();

        _missionTitleGUI.text = "Mission: " + mission.Title;
        _missionDifficultyGUI.text = "Difficulty: " + mission.Difficulty.ToString();
    }

    public void AbandonMission()
    {
        GameManager.Instance.UpdateGameState(GameState.AbandoningMission);
    }

    private void GenerateMap()
    {
        // get map from presenter
        // Show it on screen
    }
}
