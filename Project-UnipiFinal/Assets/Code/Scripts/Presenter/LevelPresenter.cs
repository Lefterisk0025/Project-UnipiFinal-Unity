using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPresenter
{
    LevelView _levelView;
    LevelLocalService _levelLocalService;
    Level _level;
    MatchConfig _matchConfig;
    GridPresenter _gridPresenter;

    public LevelPresenter(LevelView levelView)
    {
        _levelView = levelView;
        _levelLocalService = new LevelLocalService();
        _gridPresenter = new GridPresenter(_levelView.GridView);

        _levelView.OnViewInitialized.AddListener(HandleViewInitialized);
        _levelView.PreGameTimerEnded.AddListener(HandlePreGameTimerEnded);
        _levelView.LevelPerformanceView.OnLevelEndedVictorious.AddListener(HandleLevelEnded);
    }

    private void HandleViewInitialized()
    {
        // List<Level> levelsList = await _levelLocalService.LoadAllLevels();
        // int currSelectedNodeId = PlayerPrefs.GetInt("SelectedNodeId");

        // foreach (var level in levelsList)
        // {
        //     if (level.NodeId == currSelectedNodeId)
        //         _level = level;
        // }

        Debug.Log($"<color=red>HandleViewInitializedCalled!</color>");

        _level = new Level()
        {
            Grid = null,
            NodeId = 25,
            Difficulty = "Easy",
            GameMode = GameMode.MatchPoint,
        };

        if (_level.Difficulty == "Easy")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Easy);
        else if (_level.Difficulty == "Medium")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Medium);
        else if (_level.Difficulty == "Hard")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Hard);
        else if (_level.Difficulty == "Very Hard")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.VeryHard);

        _levelView.LevelPerformanceView.DisplayInitialStats(_matchConfig);

        _levelView.LevelPerformanceView.SetLevelPerformance(_matchConfig);
    }

    // From here game starts
    private void HandlePreGameTimerEnded()
    {
        // Generate grid

        if (_level.Grid == null)
        {
            _level.Grid = _gridPresenter.CreateAndInitializeGrid(_matchConfig.Height);
        }
        _levelView.GridView.InjectGridPresenter(_gridPresenter);

        if (_level.GameMode == GameMode.TimeAttack)
        {
            TimeAttackConfig timeAttackConfig = (TimeAttackConfig)_matchConfig;
            _levelView.DisplayAndStartRepeatBarTimer(timeAttackConfig.FindMatchTime);
        }

        _levelView.DisplayAndStartCentralLevelTimer(_matchConfig.TotalTime);
    }

    private void HandleLevelEnded(bool IsVictory)
    {
        _levelView.GridView.ClearGrid();
        _level = null;

        // Handle Defeat
        if (!IsVictory)
        {
            _levelView.OnLevelLost.Invoke();
            return;
        }

        // Handle Victory
        PlayerPrefs.SetInt("PreviousPointedNodeId", PlayerPrefs.GetInt("CurrentPointedNodeId"));
        PlayerPrefs.SetInt("CurrentPointedNodeId", PlayerPrefs.GetInt("SelectedNodeId"));

        GameManager.Instance.UpdateGameState(GameState.FinishingLevel);

        _levelView.OnLevelWin.Invoke();
    }
}