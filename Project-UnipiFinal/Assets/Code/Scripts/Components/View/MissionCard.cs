using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionCard : Subject
{
    public Mission Mission { get; set; }

    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private TextMeshProUGUI _difficultyTMP;
    [SerializeField] private GameObject CompletionOverlay;

    private void Awake()
    {
        CompletionOverlay.SetActive(false);
    }

    public void SetMissionCardView(Mission mission)
    {
        Mission = mission;

        _titleTMP.text = mission.Title;
        _descriptionTMP.text = mission.Description;
        _difficultyTMP.text = mission.Difficulty;

        if (mission.IsCompleted)
            CompletionOverlay.SetActive(true);
    }

    public void SelectMissionAction()
    {
        if (!Mission.IsCompleted)
            NotifyObservers(Actions.SelectMission);
    }
}
