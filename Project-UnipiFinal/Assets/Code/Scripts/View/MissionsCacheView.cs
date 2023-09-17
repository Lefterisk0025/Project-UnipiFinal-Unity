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
    [SerializeField] private bool _canFetchNewMissions;
    [SerializeField] private int _missionsCount = 5;

    [Header("General UI")]
    [SerializeField] private MissionCard _missionCardPrefab;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private Transform _missionCardsParent;
    public TextCountdownTimer MissionsRefreshTimer;

    [HideInInspector] public UnityEvent OnViewInitialized;
    [HideInInspector] public UnityEvent OnViewDisabled;

    private void Awake()
    {
        // Setup view
        for (int i = 0; i < _missionsCount; i++)
            Instantiate(_missionCardPrefab, _missionCardsParent);

        _missionsCachePresenter = new MissionsCachePresenter(this);

    }

    private void OnEnable()
    {
        OnViewInitialized.Invoke();
    }

    private void OnDisable()
    {
        OnViewDisabled.Invoke();

        StopAllCoroutines();
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

    public void DisplayTimeUntilRefresh(int timeInSec)
    {
        StartCoroutine(MissionsRefreshTimer.StartCountDownInTimeFormatHours(timeInSec));
    }

    public void OnNotify(ISubject subject, Actions action)
    {
        switch (action)
        {
            case Actions.SelectMission:
                var missionCard = (MissionCard)subject;
                InvokeSelectMission(missionCard.Mission);
                break;
        }
    }

    private void InvokeSelectMission(Mission mission)
    {
        _missionsCachePresenter.HandleMissionSelect(mission);
    }
}
