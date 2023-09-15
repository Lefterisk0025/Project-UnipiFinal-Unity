using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PlayerMatchPerformanceView : MonoBehaviour
{
    private const int _scoreAmountOnTimeAttack = 7;

    [Header("General")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private MatchResultsView _matchResultsView;

    [Header("Time attack")]
    [SerializeField] private int _totalLives = 3;
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private TMP_Text _numberOfMatchesPerTimeText;

    [Header("Match Point")]

    private int _totalMatches = 0; // Keeps track of total matches found
    private int _totalNumberOfMatchesPerTime;
    private int _scoreGoal;
    private int _currLives = 0; // Keeps track of current lives
    private int _currNumberOfMatches = 0; // Keeps track of the num of matches found before the repatable timer runs out
    private int _currScore = 0; // Keeps track of current score during match point match
    private int _scoreEarnedPerMatch = 0;
    private GameMode _currGameMode;
    private bool _isVictory = false;

    [HideInInspector] public UnityEvent OnLivesDrain;
    [HideInInspector] public UnityEvent OnAllMatchesFound;
    [HideInInspector] public UnityEvent OnScoreGoalReached;

    private void OnEnable()
    {
        ResetUI();
    }

    public void ResetUI()
    {
        _scoreText.text = "";
        _livesText.text = "";
        _numberOfMatchesPerTimeText.text = "";
    }

    public void InitializeTimeAttackPerformanceStats(int totalNumberOfMatchesPerTime)
    {
        _currGameMode = GameMode.TimeAttack;

        _currScore = 0;
        _totalNumberOfMatchesPerTime = totalNumberOfMatchesPerTime;
        _currLives = _totalLives;
        _isVictory = true;

        _scoreText.text = "Score: " + _currScore;
        _livesText.text = "Lives: " + _totalLives;
        _numberOfMatchesPerTimeText.text = $"Matches: {_currNumberOfMatches}/{_totalNumberOfMatchesPerTime}";
    }

    public void InitializeMatchPointPerformanceStats(int pointsGoal, int pointsPerMatch)
    {
        _currGameMode = GameMode.MatchPoint;

        _currScore = 0;
        _scoreGoal = pointsGoal;
        _scoreEarnedPerMatch = pointsPerMatch;
        _isVictory = false;

        _scoreText.text = $"Score: {_currScore}/{_scoreGoal}";
    }

    public void DecreaseLives()
    {
        _currLives--;

        //Update UI
        _livesText.text = "Lives: " + _currLives;

        if (_currLives <= 0)
        {
            _isVictory = false;
            Debug.Log($"<color=red>LIVES DRAIN!</color>");
            OnLivesDrain.Invoke();
        }

    }

    public void IncreaseScore()
    {
        if (_currGameMode == GameMode.TimeAttack)
        {
            _currScore += _scoreAmountOnTimeAttack;
            _scoreText.text = "Score: " + _currScore;
        }
        else if (_currGameMode == GameMode.MatchPoint)
        {
            _currScore += _scoreEarnedPerMatch;
            _scoreText.text = $"Score: {_currScore}/{_scoreGoal}";

            if (_currScore >= _scoreGoal)
            {
                _isVictory = true;
                OnScoreGoalReached.Invoke();
            }
        }

    }

    public void IncreaseNumberOfMatchesFound()
    {
        if (_currGameMode == GameMode.TimeAttack)
        {
            _currNumberOfMatches++;
            _totalMatches++;

            _numberOfMatchesPerTimeText.text = $"Matches: {_currNumberOfMatches}/{_totalNumberOfMatchesPerTime}";

            if (_currNumberOfMatches == _totalNumberOfMatchesPerTime)
            {
                OnAllMatchesFound.Invoke();
                ResetNumberOfMatchesFound();
            }
        }
        else
        {
            _totalMatches++;
        }

    }

    public void ResetNumberOfMatchesFound()
    {
        _currNumberOfMatches = 0;
        _numberOfMatchesPerTimeText.text = $"Matches: {_currNumberOfMatches}/{_totalNumberOfMatchesPerTime}";
    }

    public void OpenResultScreen()
    {
        if (_isVictory)
            PlayerPrefs.SetInt("TerminateMission", 0);
        else
            PlayerPrefs.SetInt("TerminateMission", 1);

        if (_currGameMode == GameMode.TimeAttack)
        {
            TimeAttackResults matchResults = new TimeAttackResults
            {
                IsVictory = _isVictory,
                TotalScore = _currScore,
                MatchesFound = _totalMatches,
                ReputationEarned = CalculateReputation(),
                LivesLost = _totalLives - _currLives
            };

            _matchResultsView.SetResultsScreenUi(matchResults);
            PlayerManager.Instance.UpdatePlayerMissionPerformance(matchResults);
        }
        else if (_currGameMode == GameMode.MatchPoint)
        {
            MatchPointResults matchResults = new MatchPointResults
            {
                IsVictory = _isVictory,
                TotalScore = _currScore,
                MatchesFound = _totalMatches + 1,
                ReputationEarned = CalculateReputation(),
                ScoreGoal = _scoreGoal
            };

            _matchResultsView.SetResultsScreenUi(matchResults);
            PlayerManager.Instance.UpdatePlayerMissionPerformance(matchResults);
        }
    }

    private int CalculateReputation()
    {
        if (!_isVictory)
            return 5;

        if (_currGameMode == GameMode.TimeAttack)
        {
            int scoreMultiplier = (int)Mathf.Floor(_totalMatches / _totalNumberOfMatchesPerTime);

            return 10 + (_currLives * 3) + scoreMultiplier * 2;
        }
        else
        {
            return 30;
        }
    }
}
