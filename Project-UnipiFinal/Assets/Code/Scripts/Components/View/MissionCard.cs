using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionCard : Subject
{
    public Mission Mission { get; set; }

    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private List<Image> _difficultyIcons;

    [Header("Card Background")]
    [SerializeField] private Image _cardBackground;
    [SerializeField] private Sprite _neaturalCardBackground;
    [SerializeField] private Sprite _completedCardBackground;

    [Header("Attack Button")]
    [SerializeField] private Image _attackBtn;
    [SerializeField] private Sprite _neaturalButtonBackground;
    [SerializeField] private Sprite _completedButtonBackground;

    private void Awake()
    {
        // Initialize card's UI
        InitializeCardView();
    }


    public void SetMissionCardView(Mission mission)
    {
        Mission = mission;

        _titleTMP.text = mission.Title;
        _descriptionTMP.text = mission.Description;

        int x = 0;
        if (mission.Difficulty == "Easy")
            x = 1;
        else if (mission.Difficulty == "Medium")
            x = 2;
        else if (mission.Difficulty == "Hard")
            x = 3;
        else
            x = 4;

        for (int i = 0; i < x; i++)
            _difficultyIcons[i].gameObject.SetActive(true);

        if (mission.IsCompleted)
        {
            _cardBackground.sprite = _completedCardBackground;
            _attackBtn.sprite = _completedButtonBackground;
            _attackBtn.gameObject.GetComponentInChildren<TMP_Text>().text = "COMPLETED";
            _attackBtn.gameObject.GetComponent<Button>().enabled = false;
        }
        else
        {
            InitializeCardView();
        }
    }

    private void InitializeCardView()
    {
        _cardBackground.sprite = _neaturalCardBackground;
        _attackBtn.sprite = _neaturalButtonBackground;
        _attackBtn.gameObject.GetComponentInChildren<TMP_Text>().text = "ATTACK";
        _attackBtn.gameObject.GetComponent<Button>().enabled = true;
    }

    public void SelectMissionAction()
    {
        if (!Mission.IsCompleted)
            NotifyObservers(Actions.SelectMission);
    }
}
