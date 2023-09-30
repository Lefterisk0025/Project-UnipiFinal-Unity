using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionResultsView : MonoBehaviour
{
    [SerializeField] private GameObject _resultsPanel;
    [SerializeField] private TMP_Text _outcomeText;
    [SerializeField] private TMP_Text _totalReputationText;
    [SerializeField] private TMP_Text _totalScoreText;
    [SerializeField] private TMP_Text _totalMatchesText;
    [SerializeField] private TMP_Text _bonuesReputationEarnedText;
    [SerializeField] private TMP_Text _bonusCoinsEarnedText;

    private void Start()
    {
        ClosePanelAndResetUI();
    }

    public void ClosePanelAndResetUI()
    {
        _outcomeText.text = "MISSION";
        _totalReputationText.text = "-";
        _totalScoreText.text = "-";
        _totalMatchesText.text = "-";
        _bonuesReputationEarnedText.text = "-";
        _bonusCoinsEarnedText.text = "-";

        _resultsPanel.SetActive(false);
    }

    public void DisplayResultsScreen(MissionPerformance missionPerformance)
    {
        _resultsPanel.SetActive(true);

        if (missionPerformance.IsVictory)
            _outcomeText.text = "MISSION COMPLETED";
        else
            _outcomeText.text = "MISSION FAILED";

        _totalReputationText.text = missionPerformance.TotalReputation.ToString();
        _totalScoreText.text = missionPerformance.TotalMissionScore.ToString();
        _totalMatchesText.text = missionPerformance.TotalMatches.ToString();
        _bonuesReputationEarnedText.text = missionPerformance.BonusReputation.ToString();
        _bonusCoinsEarnedText.text = missionPerformance.BonusCoins.ToString();


    }
}
