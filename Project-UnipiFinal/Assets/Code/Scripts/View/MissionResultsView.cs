using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionResultsView : MonoBehaviour
{
    [SerializeField] private GameObject _resultsPanel;
    [SerializeField] private TMP_Text _outcomeText;
    [SerializeField] private TMP_Text _reputationText;

    private void Start()
    {
        ClosePanelAndResetUI();
    }

    public void ClosePanelAndResetUI()
    {
        _outcomeText.text = "Outcome: ";
        _reputationText.text = "Total Reputation: ";
        _resultsPanel.SetActive(false);
    }

    public void DisplayResultsScreen(MissionPerformance missionPerformance)
    {
        _resultsPanel.SetActive(true);

        if (missionPerformance.IsVictory)
            _outcomeText.text = "Outcome: Victory";
        else
            _outcomeText.text = "Outcome: Defeat";

        _reputationText.text = "Total Reputation: " + missionPerformance.TotalReputation;
    }
}
