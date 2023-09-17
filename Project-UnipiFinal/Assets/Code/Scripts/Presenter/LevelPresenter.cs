using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPresenter
{
    LevelView _levelView;
    LevelLocalService _levelLocalService;
    Level _level;
    GridPresenter _gridPresenter;
    GridView _gridView;

    public LevelPresenter(LevelView levelView)
    {
        _levelView = levelView;
        _levelLocalService = new LevelLocalService();

        _levelView.OnViewInitialized.AddListener(HandleViewInitialized);
    }

    private async void HandleViewInitialized()
    {
        List<Level> levelsList = await _levelLocalService.LoadAllLevels();
        int currSelectedNodeId = PlayerPrefs.GetInt("SelectedNodeId");

        foreach (var level in levelsList)
        {
            if (level.NodeId == currSelectedNodeId)
                _level = level;
        }

        _levelView.DisplayPreGameTimer();
    }

    public void HandlePreGameTimerEnded()
    {
        MatchConfig matchConfig = null;
        if (_level.Difficulty == "Easy")
            matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Easy);
        else if (_level.Difficulty == "Medium")
            matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Medium);
        else if (_level.Difficulty == "Hard")
            matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Hard);
        else if (_level.Difficulty == "Very Hard")
            matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.VeryHard);

        TimeAttackConfig timeAttackConfig = null;
        MatchPointConfig matchPointConfig = null;
        if (matchConfig.GetType() == typeof(TimeAttackConfig))
            timeAttackConfig = (TimeAttackConfig)matchConfig;
        else if (matchConfig.GetType() == typeof(MatchPointConfig))
            matchPointConfig = (MatchPointConfig)matchConfig;

        switch (_level.GameMode)
        {
            case GameMode.TimeAttack:
                int findMatchTime = timeAttackConfig.FindMatchTime;
                int numberOfMatchesPerTime = timeAttackConfig.NumberOfMatchesPerTime;
                SetupTimeAttackMode(findMatchTime, numberOfMatchesPerTime);
                break;
            case GameMode.MatchPoint:
                int pointsGoal = matchPointConfig.ScoreGoal;
                int pointsPerMatch = matchPointConfig.PointsPerMatch;
                SetupMatchPointMode(pointsGoal, pointsPerMatch);
                break;
        }
    }
}
