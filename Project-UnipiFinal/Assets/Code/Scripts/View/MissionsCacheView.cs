using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class MissionsCacheView : MonoBehaviour, IObserver
{
    MissionsCachePresenter _missionsCachePresenter;
    MissionPresenter _missionPresenter;

    [SerializeField] private MissionCard _missionCardsPrefab;
    [SerializeField] private int _missionsCount = 5;
    [SerializeField] private bool _canFetchNewMissions = true;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private CountdownTimer _countdownTimer;

    float timeTilNextFetch = 120;

    public bool canFetch;

    private void Awake()
    {
        _missionsCachePresenter = new MissionsCachePresenter(this);
        _missionPresenter = new MissionPresenter();

        for (int i = 0; i < _missionsCount; i++)
        {
            Instantiate(_missionCardsPrefab, transform);
        }
    }

    private async void OnEnable()
    {
        try
        {
            List<Mission> missions = null;

            DateTime lastFetchDateTime = DateTime.Now;

            var lastFetchDateTimeStr = PlayerPrefs.GetString("LastFetchDateTime");
            if (!lastFetchDateTimeStr.Equals(""))
                lastFetchDateTime = DateTime.Parse(lastFetchDateTimeStr);

            if (!_missionsCachePresenter.CanFetchNewMissions(lastFetchDateTime, DateTime.Now))
            {
                missions = await _missionsCachePresenter.GetLocalMissionsCacheData();
                _messageText.text = "Missions already fetched. Back in 2 hours...";

                //timeTilNextFetch = CalculateElapsedTimeInMinutes(lastFetchDateTime, DateTime.Now);
            }
            else
            {
                //missions = await _missionsPresenter.GetRandomRemoteMissions(_missionsCount);
                missions = _missionsCachePresenter.GetRandomLocalMissions(_missionsCount);
                _missionsCachePresenter.SaveLocalMissionsCacheData(missions);
                PlayerPrefs.SetString("LastFetchDateTime", DateTime.Now.ToString());
                _messageText.text = "Just fetched new missions!";

                //timeTilNextFetch = 120;
            }

            //_countdownTimer.SetTimer(timeTilNextFetch);

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
        catch (Exception e)
        {
            ErrorScreen.Instance.Show(e.Message);
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
        LoadingScreen.Instance.FakeOpen(2);

        _missionPresenter.SaveLocalMissionData(mission);

        GameManager.Instance.UpdateGameState(GameState.InitializingMission);
    }

    private float CalculateElapsedTimeInMinutes(DateTime dt1, DateTime dt2)
    {
        TimeSpan elapsedTime = dt2 - dt1;
        float hours = (float)elapsedTime.TotalHours;
        return hours * 60;
    }
}
