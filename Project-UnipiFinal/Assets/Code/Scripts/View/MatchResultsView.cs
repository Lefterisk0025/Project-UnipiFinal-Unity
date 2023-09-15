using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchResultsView : MonoBehaviour
{
    [SerializeField] private GameObject _resultsPanel;
    [SerializeField] private TMP_Text _resultHeader;
    [SerializeField] private TMP_Text _totalScoreText;
    [SerializeField] private TMP_Text _matchesFoundText;
    [SerializeField] private TMP_Text _livesLostText;
    [SerializeField] private TMP_Text _reputationEarnedText;

    private void Start()
    {
        ResetUI();
    }

    public void ResetUI()
    {
        _resultHeader.text = "";
        _totalScoreText.text = "";
        _matchesFoundText.text = "";
        _livesLostText.text = "";
        _reputationEarnedText.text = "";
        _resultsPanel.SetActive(false);
    }

    public void SetResultsScreenUi(MatchResults matchResults)
    {
        _resultsPanel.SetActive(true);

        if (matchResults.IsVictory)
            _resultHeader.text = $"<color=#009510>Victory!</color>";
        else
            _resultHeader.text = $"<color=red>Defeat</color>";

        _matchesFoundText.text = "Matches Found: " + matchResults.MatchesFound.ToString();
        _reputationEarnedText.text = $"+{matchResults.ReputationEarned.ToString()} Reputation";

        if (matchResults.GetType() == typeof(TimeAttackResults))
        {
            var timeAttackResults = (TimeAttackResults)matchResults;

            _livesLostText.text = "Lives Lost: " + timeAttackResults.LivesLost.ToString();
        }
        else if (matchResults.GetType() == typeof(MatchPointResults))
        {
            var matchPointResults = (MatchPointResults)matchResults;

            _totalScoreText.text = $"Score: {matchResults.TotalScore.ToString()}/{matchPointResults.ScoreGoal.ToString()}";
        }

        Time.timeScale = 0;
    }
}
