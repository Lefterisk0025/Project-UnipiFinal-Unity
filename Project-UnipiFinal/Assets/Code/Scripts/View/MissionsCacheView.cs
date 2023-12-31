using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;

public class MissionsCacheView : MonoBehaviour, IObserver
{
    MissionsCachePresenter _missionsCachePresenter;

    [Header("Settings")]
    [SerializeField] private int _missionsCount = 5;
    [SerializeField] private int _refreshPrice = 500;

    [Header("General UI")]
    [SerializeField] private MissionCard _missionCardPrefab;
    [SerializeField] private Transform _missionCardsParent;
    [SerializeField] private TextCountdownTimer _missionsRefreshTimer;

    [HideInInspector] public UnityEvent OnViewInitialized;
    [HideInInspector] public UnityEvent OnViewDisabled;
    [HideInInspector] public UnityEvent OnMissionsRefreshTimerEnded;

    private void Awake()
    {
        // Setup view
        for (int i = 0; i < _missionsCount; i++)
            Instantiate(_missionCardPrefab, _missionCardsParent);

        _missionsCachePresenter = new MissionsCachePresenter(this);
    }

    private void OnEnable()
    {
        _missionsRefreshTimer.OnTimerEnd.AddListener(() => OnMissionsRefreshTimerEnded.Invoke());
        PlayerManager.Instance.HideAvatar();
        OnViewInitialized.Invoke();
    }

    private void OnDisable()
    {
        _missionsRefreshTimer.OnTimerEnd.RemoveAllListeners();
        StopAllCoroutines();

        OnViewDisabled.Invoke();
    }

    public void OnRefreshMissionsButtonClicked()
    {
        AlertWindow.Instance.ShowYesNoAlert("Refresh Missions", $"Are you sure you would like to spend {_refreshPrice} coins?", () =>
        {
            if (PlayerManager.Instance.Player.NetCoins >= _refreshPrice)
            {
                PlayerManager.Instance.BuyItem(_refreshPrice);
                StopAllCoroutines();
                _missionsCachePresenter.HandleRefreshMissionsButtonClicked();
            }
            else
            {
                AlertWindow.Instance.ShowMessageAlert("Problem", "You don't have enough coins!");
            }
        }, () => { });
    }

    public void DisplayMissions(List<Mission> missionsList)
    {
        int i = 0;
        foreach (Transform item in _missionCardsParent.transform)
        {
            var cardView = item.GetComponent<MissionCard>();
            cardView.SetMissionCardView(missionsList[i]);
            cardView.AddObserver(this);
            i++;
        }
    }

    public void ClearMissionCards()
    {
        foreach (Transform cardGO in _missionCardsParent)
        {
            cardGO.GetComponent<MissionCard>().RemoveObserver(this);
        }
    }

    public void DisplayTimeUntilMissionsRefresh(int timeInSec)
    {
        StartCoroutine(_missionsRefreshTimer.StartCountDownInTimeFormatHours(timeInSec));
    }

    public void OnNotify(ISubject subject, Actions action)
    {
        switch (action)
        {
            case Actions.SelectMission:
                var missionCard = (MissionCard)subject;
                HandleSelectMission(missionCard.Mission);
                break;
        }
    }

    private void HandleSelectMission(Mission mission)
    {
        _missionsCachePresenter.HandleMissionSelect(mission);
    }
}
