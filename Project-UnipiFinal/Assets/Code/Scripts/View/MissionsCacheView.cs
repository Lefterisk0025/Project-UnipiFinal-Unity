using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsCacheView : MonoBehaviour, IObserver
{
    MissionsCachePresenter _missionsCachePresenter;

    [SerializeField] private MissionCard _missionCardsPrefab;
    [SerializeField] private int _missionsCount = 5;
    [SerializeField] private bool _canFetchNewMissions = true;

    private void Awake()
    {
        _missionsCachePresenter = new MissionsCachePresenter(this);

        for (int i = 0; i < _missionsCount; i++)
        {
            Instantiate(_missionCardsPrefab, transform);
        }
    }

    private void OnEnable()
    {
        // ADD CHECK FOR GETTING NEW MISSIONS EVERY 2 HOURS
        List<Mission> missions;

        if (!_canFetchNewMissions)
            missions = _missionsCachePresenter.GetCurrentMissions();
        else
            missions = _missionsCachePresenter.GetNewRandomMissions(_missionsCount);

        if (missions == null || missions.Count != _missionsCount)
        {
            Debug.Log("An error occured while fetching missions.");
            return;
        }

        int i = 0;
        foreach (Transform childGO in transform)
        {
            var missionCard = childGO.GetComponent<MissionCard>();
            missionCard.SetMissionCardView(missions[i]);
            missionCard.AddObserver(this);
            i++;
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
        LoadingScreen.Instance.Open(2);

        _missionsCachePresenter.SaveMissionDataLocal(mission);

        GameManager.Instance.UpdateGameState(GameState.InitializingMission);
    }
}
