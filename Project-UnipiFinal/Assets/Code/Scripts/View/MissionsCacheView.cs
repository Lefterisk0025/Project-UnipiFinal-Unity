using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsCacheView : MonoBehaviour, IObserver
{
    MissionsCachePresenter _missionsCachePresenter;

    [SerializeField] private List<MissionCard> _missionCardsList;
    [SerializeField] private MissionDataSO _missionDataSO;

    private void Awake()
    {
        _missionsCachePresenter = new MissionsCachePresenter(this);
    }

    // OnEnable its being called after Awake() and before Start()
    private void OnEnable()
    {
        List<Mission> missionsList = _missionsCachePresenter.GetRandomMissions(_missionCardsList.Count);

        int i = 0;
        foreach (Mission mission in missionsList)
        {
            if (_missionCardsList[i] == null)
                break;

            _missionCardsList[i].SetMissionCardView(mission);
            _missionCardsList[i].AddObserver(this);
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
        _missionDataSO.Difficulty = mission.Difficulty;
        _missionDataSO.Title = mission.Title;

        GameManager.Instance.UpdateGameState(GameState.InitializingMission);
    }
}
