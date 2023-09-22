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

    int _currLevelIndex;
    List<Level> _levelsList;
    bool _isVictory;

    public LevelPresenter(LevelView levelView)
    {
        _levelView = levelView;
        _levelLocalService = new LevelLocalService();
        _gridPresenter = new GridPresenter(_levelView.GridView);

        // Setup events
        _levelView.OnViewInitialized.AddListener(HandleViewInitialized);
    }

    private async void HandleViewInitialized()
    {
        _levelView.PreGameTimerEnded.AddListener(HandlePreGameTimerEnded);
        _levelView.OnLevelEnd.AddListener(HandleLevelEnded);

        LoadingScreen.Instance.Open();

        _levelsList = new List<Level>(await _levelLocalService.LoadAllLevels());
        int currSelectedNodeId = PlayerPrefs.GetInt("SelectedNodeId");

        _currLevelIndex = 0;
        foreach (var level in _levelsList)
        {
            if (level.NodeId == currSelectedNodeId)
            {
                _level = level;
                break;
            }
            _currLevelIndex++;
        }

        if (_level.Difficulty == "Easy")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Easy);
        else if (_level.Difficulty == "Medium")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Medium);
        else if (_level.Difficulty == "Hard")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.Hard);
        else if (_level.Difficulty == "Very Hard")
            _matchConfig = _levelView.GetMatchConfigByDifficulty(_level.GameMode, Difficulty.VeryHard);

        if (_level.GameMode == GameMode.TimeAttack)
            _levelView.DisplayRepeatBarTimer();

        _levelView.DisplayCentralLevelTimer(_matchConfig.TotalTime);

        _levelView.LevelPerformanceView.DisplayInitialStats(_matchConfig);

        _levelView.LevelPerformanceView.SetLevelPerformance(_matchConfig);

        LoadingScreen.Instance.CloseAfterDelay(1);
    }

    // From here game starts
    private async void HandlePreGameTimerEnded()
    {
        // Generate grid
        if (_level.Grid == null)
        {
            _level.Grid = _gridPresenter.CreateAndInitializeGrid(_matchConfig.Height);
            _levelsList[_currLevelIndex].Grid = _level.Grid;
            await _levelLocalService.SaveAllLevels(_levelsList);
        }
        else
        {
            _gridPresenter.InitializeGrid(_level.Grid);
        }
        _levelView.GridView.InjectGridPresenter(_gridPresenter);

        // Start the timers
        if (_level.GameMode == GameMode.TimeAttack)
            _levelView.StartRepeatBarTimer(((TimeAttackConfig)_matchConfig).FindMatchTime);

        _levelView.StartCentralLevelTimer(_matchConfig.TotalTime);
    }

    private void HandleLevelEnded(bool isVictory)
    {
        _isVictory = isVictory;
    }

    public void HandleAbandonLevel()
    {
        ClearLevel();

        _levelView.OnMissionEnd.Invoke(_isVictory);
        PlayerManager.Instance.DisplayMissionResults(_isVictory);
    }

    public void HandleContinueLevel()
    {
        ClearLevel();

        PlayerPrefs.SetInt("PreviousPointedNodeId", PlayerPrefs.GetInt("CurrentPointedNodeId"));
        PlayerPrefs.SetInt("CurrentPointedNodeId", PlayerPrefs.GetInt("SelectedNodeId"));

        GameManager.Instance.UpdateGameState(GameState.FinishingLevel);

        _levelView.OnPointedTargetChanged.Invoke();
    }

    private void ClearLevel()
    {
        _levelView.GridView.ClearGrid();
        _level = null;

        _levelView.DisableTimers();

        _levelView.PreGameTimerEnded.RemoveAllListeners();
        _levelView.CentralLevelTimerEnded.RemoveAllListeners();
        _levelView.RepeatBarTimerEnded.RemoveAllListeners();
        _levelView.LevelPerformanceView.OnAllMatchesFound.RemoveAllListeners();
    }

    public void HandleOnMissionEnded(bool isVictory)
    {
        if (!isVictory || (PlayerPrefs.GetInt("IsFinalNode") == 1))
        {
            PlayerManager.Instance.DisplayMissionResults(isVictory);
            return;
        }
    }
}
