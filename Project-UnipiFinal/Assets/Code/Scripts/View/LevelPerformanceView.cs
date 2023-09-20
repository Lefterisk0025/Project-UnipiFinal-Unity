using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class LevelPerformanceView : MonoBehaviour
{
    LevelPerformancePresenter _levelPerformancePresenter;

    [Header("General Settings")]
    [SerializeField] internal LevelView LevelView;

    [Header("Performance GUI")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private TMP_Text _numberOfMatchesPerTimeText;

    [Header("Results GUI")]
    [SerializeField] private GameObject _resultsPanel;
    [SerializeField] private TMP_Text _resultHeader;
    [SerializeField] private TMP_Text _totalScoreText;
    [SerializeField] private TMP_Text _matchesFoundText;
    [SerializeField] private TMP_Text _livesLostText;
    [SerializeField] private TMP_Text _reputationEarnedText;

    [HideInInspector] public UnityEvent OnAllMatchesFound;
    [HideInInspector] public UnityEvent<bool> OnLevelEndedVictorious;

    // Starting point of Level Performance view
    public void SetLevelPerformance(MatchConfig matchConfig)
    {
        _levelPerformancePresenter = new LevelPerformancePresenter(this);
        _levelPerformancePresenter.HandleLevelPerformanceSet(matchConfig);
        ResetResultsPanel();
        _resultsPanel.SetActive(false);
    }

    private void OnDisable() { }

    private void ResetResultsPanel()
    {
        _resultHeader.text = "";
        _totalScoreText.text = "";
        _matchesFoundText.text = "";
        _livesLostText.text = "";
        _reputationEarnedText.text = "";
    }

    public void DisplayInitialStats(MatchConfig config)
    {
        if (config is TimeAttackConfig timeAttackConfig)
        {
            _numberOfMatchesPerTimeText.text = $"Matches: 0/{timeAttackConfig.NumberOfMatchesPerTime}";
            _scoreText.text = $"Score: 0";
            _livesText.text = $"Lives: {TimeAttackPerformance.MaxLives}/{TimeAttackPerformance.MaxLives}";
        }
        else if (config is MatchPointConfig matchPointConfig)
        {
            _scoreText.text = $"Score: 9/{matchPointConfig.ScoreGoal}";
        }
    }

    public void DisplayTimeAttackStats(TimeAttackPerformance performance, TimeAttackConfig config)
    {
        _numberOfMatchesPerTimeText.text = $"Matches: {performance.CurrentMatches}/{config.NumberOfMatchesPerTime}";
        _scoreText.text = $"Score: {performance.TotalScore}";
        _livesText.text = $"Lives: {performance.CurrentLives}/{TimeAttackPerformance.MaxLives}";
    }

    public void DisplayMatchPointStats(MatchPointPerformance performance, MatchPointConfig config)
    {
        _scoreText.text = $"Score: {performance.TotalScore}/{config.ScoreGoal}";
    }

    public void DisplayPerformanceResults(LevelPerformance performance)
    {
        _resultsPanel.SetActive(true);

        if (performance.IsVictory)
            _resultHeader.text = $"<color=#009510>Victory!</color>";
        else
            _resultHeader.text = $"<color=red>Defeat</color>";

        _matchesFoundText.text = "Matches Found: " + performance.TotalMatches.ToString();
        _reputationEarnedText.text = $"+{performance.ReputationEarned} Reputation";

        if (performance is TimeAttackPerformance timeAttackPerformance)
        {
            _totalScoreText.text = $"Score: {performance.TotalScore}";

            int livesLost = TimeAttackPerformance.MaxLives - timeAttackPerformance.CurrentLives;
            _livesLostText.text = "Lives Lost: " + livesLost.ToString();
        }
        else if (performance is MatchPointPerformance matchPointPerformance)
        {
            _totalScoreText.text = $"Score: {matchPointPerformance.TotalScore}/{matchPointPerformance.ScoreGoal}";
        }

        Time.timeScale = 0;
    }

    public void OnEndLevelButtonClicked()
    {
        Time.timeScale = 1;

        _levelPerformancePresenter.HandleEndLevelButtonClicked();
    }
}
