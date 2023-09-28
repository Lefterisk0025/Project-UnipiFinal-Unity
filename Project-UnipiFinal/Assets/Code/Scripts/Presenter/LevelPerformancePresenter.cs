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
    }

    public void HandleLevelPerformanceSet(MatchConfig matchConfig)
    {
        // Setup events used by all game modes
        _levelPerformanceView.LevelView.CentralLevelTimerEnded.AddListener(HandleCentralLevelTimerEnded);
        _levelPerformanceView.LevelView.GridView.OnMatchFound.AddListener(HandleMatchFound);
        _levelPerformanceView.LevelView.OnLevelEnd.AddListener(HandleLevelEnded);

        if (matchConfig is TimeAttackConfig timeAttackConfig)
        {
            // Initialize private scope variables
            _currGameMode = GameMode.TimeAttack;
            _timeAttackConfig = timeAttackConfig;
            _timeAttackPerformance = new TimeAttackPerformance()
            {
                TotalScore = 0,
                TotalMatches = 0,
                CurrentLives = 3,
                CurrentMatches = 0,
            };
            // Setup events for Time Attack GameMode
            _levelPerformanceView.LevelView.RepeatBarTimerEnded.AddListener(HandleRepeatBarTimerEnded);
        }
        else if (matchConfig is MatchPointConfig matchPointConfig)
        {
            _currGameMode = GameMode.MatchPoint;
            _matchPointConfig = matchPointConfig;
            _matchPointPerformance = new MatchPointPerformance()
            {
                TotalScore = 0,
                TotalMatches = 0,
                ScoreGoal = matchPointConfig.ScoreGoal,
            };
            // Setup events for Match Point GameMode
        }
    }

    private void HandleCentralLevelTimerEnded()
    {
        if (_currGameMode == GameMode.TimeAttack)
            _levelPerformanceView.LevelView.OnLevelEnd.Invoke(true);
        else if (_currGameMode == GameMode.MatchPoint)
        {
            if (_matchPointPerformance.TotalScore >= _matchPointConfig.ScoreGoal)
                _levelPerformanceView.LevelView.OnLevelEnd.Invoke(true);
            else
                _levelPerformanceView.LevelView.OnLevelEnd.Invoke(false);
        }
    }

    private void HandleRepeatBarTimerEnded()
    {
        Debug.Log($"<color=green>HandleRepeatBarTimerEnded called</color>");
        _timeAttackPerformance.CurrentLives--;
        _timeAttackPerformance.CurrentMatches = 0;

        _levelPerformanceView.DisplayTimeAttackStats(_timeAttackPerformance, _timeAttackConfig);

        if (_timeAttackPerformance.CurrentLives <= 0)
        {
            _levelPerformanceView.LevelView.OnLevelEnd.Invoke(false);
        }
    }

    private void HandleMatchFound()
    {
        Debug.Log("CurrGameMode: " + _currGameMode);
        if (_currGameMode == GameMode.TimeAttack)
        {
            _timeAttackPerformance.CurrentMatches++;
            _timeAttackPerformance.TotalMatches++;
            _timeAttackPerformance.TotalScore += 7;

            _levelPerformanceView.DisplayTimeAttackStats(_timeAttackPerformance, _timeAttackConfig);

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

            _levelPerformanceView.DisplayMatchPointStats(_matchPointPerformance, _matchPointConfig);

            if (_matchPointPerformance.TotalScore == _matchPointConfig.ScoreGoal)
            {
                _levelPerformanceView.LevelView.OnLevelEnd.Invoke(true);
            }
        }
    }

    public void HandleLevelEnded(bool isVictory)
    {
        switch (_currGameMode)
        {
            case GameMode.TimeAttack:
                _timeAttackPerformance.IsVictory = isVictory;
                _timeAttackPerformance.ReputationEarned = CalculateAndGetReputation();
                _levelPerformanceView.DisplayPerformanceResults(_timeAttackPerformance);
                PlayerManager.Instance.SetPlayerMissionStats(_timeAttackPerformance.TotalScore, _timeAttackPerformance.ReputationEarned, _timeAttackPerformance.TotalMatches);
                break;
            case GameMode.MatchPoint:
                _matchPointPerformance.IsVictory = isVictory;
                _matchPointPerformance.ReputationEarned = CalculateAndGetReputation();
                _levelPerformanceView.DisplayPerformanceResults(_matchPointPerformance);
                PlayerManager.Instance.SetPlayerMissionStats(_matchPointPerformance.TotalScore, _matchPointPerformance.ReputationEarned, _matchPointPerformance.TotalMatches);
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
