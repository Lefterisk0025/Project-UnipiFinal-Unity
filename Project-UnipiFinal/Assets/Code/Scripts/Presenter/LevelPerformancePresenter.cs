using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelPerformancePresenter
{
    LevelPerformanceView _levelPerformanceView;
    MatchPointConfig _matchPointConfig;
    TimeAttackConfig _timeAttackConfig;
    TimeAttackPerformance _timeAttackPerformance;
    MatchPointPerformance _matchPointPerformance;

    GameMode _currGameMode;

    public LevelPerformancePresenter(LevelPerformanceView levelPerformanceView)
    {
        _levelPerformanceView = levelPerformanceView;

        _levelPerformanceView.LevelView.PreGameTimerEnded.AddListener(HandlePreGameTimerEnded);
        _levelPerformanceView.OnLevelEndedVictorious.AddListener(HandleLevelEnded);
    }

    public void HandleLevelPerformanceSet(MatchConfig matchConfig)
    {
        if (matchConfig is TimeAttackConfig timeAttack)
        {
            _currGameMode = GameMode.TimeAttack;
            _timeAttackConfig = timeAttack;
            _timeAttackPerformance = new TimeAttackPerformance()
            {
                TotalScore = 0,
                TotalMatches = 0,
                CurrentLives = TimeAttackPerformance.MaxLives,
                CurrentMatches = 0,
            };
            SetupTimeAttack();
        }
        else if (matchConfig is MatchPointConfig matchPoint)
        {
            _currGameMode = GameMode.MatchPoint;
            _matchPointConfig = matchPoint;
            _matchPointPerformance = new MatchPointPerformance()
            {
                TotalScore = 0,
                TotalMatches = 0,
                ScoreGoal = matchPoint.ScoreGoal,
            };
            SetupMatchPoint();
        }
    }

    private void SetupTimeAttack()
    {
        // Initialize UI
        UpdateView();

        // Initialize Events
        _levelPerformanceView.LevelView.CentralLevelTimerEnded.AddListener(HandleCentralLevelTimerEnded);
        _levelPerformanceView.LevelView.RepeatBarTimerEnded.AddListener(HandleRepeatBarTimerEnded);
        _levelPerformanceView.LevelView.GridView.OnMatchFound.AddListener(HandleMatchFound);
    }

    private void SetupMatchPoint()
    {
        // Initialize UI
        UpdateView();

        _levelPerformanceView.LevelView.CentralLevelTimerEnded.AddListener(HandleCentralLevelTimerEnded);
        _levelPerformanceView.LevelView.GridView.OnMatchFound.AddListener(HandleMatchFound);
    }

    private void HandlePreGameTimerEnded()
    {
        UpdateView();
    }

    private void HandleCentralLevelTimerEnded()
    {
        // Show results screen
        switch (_currGameMode)
        {
            case GameMode.TimeAttack:
                _timeAttackPerformance.IsVictory = true;
                _timeAttackPerformance.ReputationEarned = CalculateAndGetReputation();
                _levelPerformanceView.DisplayPerformanceResults(_timeAttackPerformance);
                break;
            case GameMode.MatchPoint:
                if (_matchPointPerformance.TotalScore >= _matchPointConfig.ScoreGoal)
                    _matchPointPerformance.IsVictory = true;
                _matchPointPerformance.ReputationEarned = CalculateAndGetReputation();
                _levelPerformanceView.DisplayPerformanceResults(_matchPointPerformance);
                break;
        }
    }

    private void HandleRepeatBarTimerEnded()
    {
        _timeAttackPerformance.CurrentLives--;
        _timeAttackPerformance.CurrentMatches = 0;

        UpdateView();

        if (_timeAttackPerformance.CurrentLives <= 0)
        {
            _timeAttackPerformance.ReputationEarned = CalculateAndGetReputation();
            _levelPerformanceView.DisplayPerformanceResults(_timeAttackPerformance);
        }
    }

    private void HandleMatchFound()
    {
        if (_currGameMode == GameMode.TimeAttack)
        {
            _timeAttackPerformance.CurrentMatches++;
            _timeAttackPerformance.TotalMatches++;
            _timeAttackPerformance.TotalScore += 7;

            UpdateView();

            if (_timeAttackPerformance.CurrentMatches == _timeAttackConfig.NumberOfMatchesPerTime)
            {
                _levelPerformanceView.OnAllMatchesFound.Invoke();
                _timeAttackPerformance.CurrentMatches = 0;
            }
        }
        else if (_currGameMode == GameMode.MatchPoint)
        {
            _matchPointPerformance.TotalMatches++;
            _matchPointPerformance.TotalScore += _matchPointConfig.PointsPerMatch;

            UpdateView();

            if (_matchPointPerformance.TotalScore == _matchPointConfig.ScoreGoal)
            {
                _matchPointPerformance.IsVictory = true;
                _matchPointPerformance.ReputationEarned = CalculateAndGetReputation();
                _levelPerformanceView.DisplayPerformanceResults(_matchPointPerformance);
            }
        }
    }

    public void HandleEndLevelButtonClicked()
    {
        switch (_currGameMode)
        {
            case GameMode.TimeAttack:
                _levelPerformanceView.OnLevelEndedVictorious.Invoke(_timeAttackPerformance.IsVictory);
                break;
            case GameMode.MatchPoint:
                _levelPerformanceView.OnLevelEndedVictorious.Invoke(_matchPointPerformance.IsVictory);
                break;
        }

    }

    private void HandleLevelEnded(bool IsVictory)
    {
        _levelPerformanceView.LevelView.CentralLevelTimerEnded.RemoveListener(HandleCentralLevelTimerEnded);
        _levelPerformanceView.LevelView.RepeatBarTimerEnded.RemoveListener(HandleRepeatBarTimerEnded);
        _levelPerformanceView.LevelView.GridView.OnMatchFound.RemoveListener(HandleMatchFound);
    }

    private void UpdateView()
    {
        switch (_currGameMode)
        {
            case GameMode.TimeAttack:
                _levelPerformanceView.DisplayTimeAttackStats(_timeAttackPerformance, _timeAttackConfig);
                break;
            case GameMode.MatchPoint:
                _levelPerformanceView.DisplayMatchPointStats(_matchPointPerformance, _matchPointConfig);
                break;
        }
    }

    private int CalculateAndGetReputation()
    {
        if (_currGameMode == GameMode.TimeAttack)
        {
            if (!_timeAttackPerformance.IsVictory)
                return 5;

            int scoreMultiplier = (int)Mathf.Floor(_timeAttackPerformance.TotalMatches / _timeAttackConfig.NumberOfMatchesPerTime);

            return 10 + (_timeAttackPerformance.CurrentLives * 3) + scoreMultiplier * 2;
        }
        else
        {
            if (!_matchPointPerformance.IsVictory)
                return 5;

            return 30;
        }
    }
}
