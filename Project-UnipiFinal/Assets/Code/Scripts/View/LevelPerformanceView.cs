using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _abandonBtn;
    [SerializeField] private Button _finishBtn;

    [HideInInspector] public UnityEvent OnAllMatchesFound;

    // Starting point of Level Performance view
    public void SetLevelPerformance(MatchConfig matchConfig)
    {
        _levelPerformancePresenter = new LevelPerformancePresenter(this);
        _levelPerformancePresenter.HandleLevelPerformanceSet(matchConfig);
    }

    private void OnEnable()
    {
        ResetResultsPanel();
        ResetPerformanceStatsUI();
    }

    private void ResetResultsPanel()
    {
        _resultHeader.text = "";
        _totalScoreText.text = "";
        _matchesFoundText.text = "";
        _livesLostText.text = "";
        _reputationEarnedText.text = "";
        _continueBtn.gameObject.SetActive(false);
        _abandonBtn.gameObject.SetActive(false);
        _finishBtn.gameObject.SetActive(false);
        _resultsPanel.SetActive(false);
    }

    private void ResetPerformanceStatsUI()
    {
        _numberOfMatchesPerTimeText.text = "";
        _scoreText.text = "";
        _livesText.text = "";
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
            _scoreText.text = $"Score: 0/{matchPointConfig.ScoreGoal}";
        }
    }

    public void DisplayTimeAttackStats(TimeAttackPerformance performance, TimeAttackConfig config)
    {
        Debug.Log("I am in John...");
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
        ResetPerformanceStatsUI();

        _resultsPanel.SetActive(true);

        Debug.Log("IsFinalNode: " + PlayerPrefs.GetInt("IsFinalNode"));

        if (performance.IsVictory)
        {
            if (PlayerPrefs.GetInt("IsFinalNode") == 1)
                _finishBtn.gameObject.SetActive(true);
            else
                _continueBtn.gameObject.SetActive(true);

            _resultHeader.text = $"<color=#009510>Victory!</color>";
        }
        else
        {
            _abandonBtn.gameObject.SetActive(true);
            _resultHeader.text = $"<color=red>Defeat</color>";
        }

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

    public void OnCloseLevelButtonClicked()
    {
        //Time.timeScale = 1;

        //_levelPerformancePresenter.HandleCloseLevelButtonClicked();
    }

    public void OnAbandonLevelButtonClicked()
    {
        Time.timeScale = 1;

        LevelView.AbandonLevel();
    }

    public void OnContinueLevelButtonClicked()
    {
        Time.timeScale = 1;

        LevelView.ContinueLevel();
    }
}
