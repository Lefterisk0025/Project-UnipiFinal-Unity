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
    MissionPresenter _missionPresenter;

    [Header("Settings")]
    [SerializeField] private bool _canFetchNewMissions;
    [SerializeField] private int _missionsCount = 5;

    [Header("General UI")]
    [SerializeField] private MissionCard _missionCardPrefab;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private Transform _missionCardsParent;

    [HideInInspector] public UnityEvent<int, bool> OnViewInitialized;
    [HideInInspector] public UnityEvent OnViewDisabled;

    private void Awake()
    {
        // Setup view
        for (int i = 0; i < _missionsCount; i++)
            Instantiate(_missionCardPrefab, _missionCardsParent);
    }

    private void OnEnable()
    {
        _missionsCachePresenter = new MissionsCachePresenter(this);

        OnViewInitialized.Invoke(_missionsCount, _canFetchNewMissions);
    }

    private void OnDisable()
    {
        OnViewDisabled.Invoke();
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

    public void OnNotify(ISubject subject, Actions action)
    {
        switch (action)
        {
            case Actions.SelectMission:
                var missionCard = (MissionCard)subject;
                HandleSelectMissionAction(missionCard.Mission);
                break;
        }
    }

    private void HandleSelectMissionAction(Mission mission)
    {
        Debug.Log("Mission with title " + mission.Title + " selected!");
    }
}
