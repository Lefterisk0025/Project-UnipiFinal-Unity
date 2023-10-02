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
    [SerializeField] private GameObject _timeAttackStatsParent;
    [SerializeField] private GameObject _matchPointStatsParent;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private TMP_Text _numberOfMatchesPerTimeText;

    [Header("Results GUI")]
    [SerializeField] private GameObject _resultsPanel;
    [SerializeField] private TMP_Text _resultHeader;
    [SerializeField] private Transform _totalScore;
    [SerializeField] private Transform _matchesFound;
    [SerializeField] private Transform _livesLost;
    [SerializeField] private Transform _reputationEarned;
    [SerializeField] private Transform _coinsEarned;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _abandonBtn;

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

        _resultsPanel.SetActive(false);
    }

    private void ResetResultsPanel()
    {
        _resultHeader.text = "";
        _totalScore.Find("Value").GetComponent<TMP_Text>().text = "0";
        _matchesFound.Find("Value").GetComponent<TMP_Text>().text = "0";
        _livesLost.Find("Value").GetComponent<TMP_Text>().text = "0";
        _reputationEarned.Find("Value").GetComponent<TMP_Text>().text = "0";
        _continueBtn.gameObject.SetActive(false);
        _abandonBtn.gameObject.SetActive(false);
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
        _timeAttackStatsParent.SetActive(false);
        _matchPointStatsParent.SetActive(false);

        if (config is TimeAttackConfig timeAttackConfig)
        {
            _timeAttackStatsParent.SetActive(true);
            _numberOfMatchesPerTimeText.text = $"0/{timeAttackConfig.NumberOfMatchesPerTime}";
            _livesText.text = $"{TimeAttackPerformance.MaxLives}/{TimeAttackPerformance.MaxLives}";
        }
        else if (config is MatchPointConfig matchPointConfig)
        {
            _matchPointStatsParent.SetActive(true);
            _scoreText.text = $"0/{matchPointConfig.ScoreGoal}";
        }
    }

    public void DisplayTimeAttackStats(TimeAttackPerformance performance, TimeAttackConfig config)
    {
        _numberOfMatchesPerTimeText.text = $"{performance.CurrentMatches}/{config.NumberOfMatchesPerTime}";
        _livesText.text = $"{performance.CurrentLives}/{TimeAttackPerformance.MaxLives}";
    }

    public void DisplayMatchPointStats(MatchPointPerformance performance, MatchPointConfig config)
    {
        _scoreText.text = $"{performance.TotalScore}/{config.ScoreGoal}";
    }

    public void DisplayPerformanceResults(LevelPerformance performance)
    {
        _livesLost.gameObject.SetActive(false);

        _resultsPanel.SetActive(true);

        if (performance.IsVictory)
        {
            if (PlayerPrefs.GetInt("IsFinalNode") == 1)
                _abandonBtn.gameObject.SetActive(true);
            else
                _continueBtn.gameObject.SetActive(true);

            _resultHeader.text = $"<color=#00F11A>COMPLETED!</color>";
        }
        else
        {
            _abandonBtn.gameObject.SetActive(true);
            _resultHeader.text = $"<color=#FF5F45>GAME OVER</color>";
        }

        _matchesFound.Find("Value").GetComponent<TMP_Text>().text = performance.TotalMatches.ToString();

        if (performance is TimeAttackPerformance timeAttackPerformance)
        {
            _totalScore.Find("Value").GetComponent<TMP_Text>().text = performance.TotalScore.ToString();

            _livesLost.gameObject.SetActive(true);
            int livesLost = TimeAttackPerformance.MaxLives - timeAttackPerformance.CurrentLives;
            _livesLost.Find("Value").GetComponent<TMP_Text>().text = livesLost.ToString();
        }
        else if (performance is MatchPointPerformance matchPointPerformance)
        {
            _totalScore.Find("Value").GetComponent<TMP_Text>().text = $"{matchPointPerformance.TotalScore}/{matchPointPerformance.ScoreGoal}";
        }

        _reputationEarned.Find("Value").GetComponent<TMP_Text>().text = performance.ReputationEarned.ToString();
        _coinsEarned.Find("Value").GetComponent<TMP_Text>().text = performance.CoinsEarned.ToString();

        Time.timeScale = 0;
    }
}
