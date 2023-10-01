using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

public class MissionsCachePresenter
{
    const int _missionsCount = 5;
    const float _refreshTimeInHours = 2.0f;

    MissionLocalService _missionLocalService;
    MissionRemoteService _missionRemoteService;
    MissionsCacheView _missionsCacheView;
    List<Mission> _missionsList;

    bool isFetching = false;

    public MissionsCachePresenter(MissionsCacheView missionsCacheView)
    {
        _missionsCacheView = missionsCacheView;
        _missionLocalService = new MissionLocalService();
        _missionRemoteService = new MissionRemoteService();
        _missionsList = new List<Mission>();

        _missionsCacheView.OnViewInitialized.AddListener(HandleViewInitialized);
        _missionsCacheView.OnMissionsRefreshTimerEnded.AddListener(HandleMissionRefreshTimerEnded);
    }

    public async void HandleViewInitialized()
    {
        DateTime currDateTime = DateTime.Now;
        DateTime lastFetchDateTime = DateTime.Now;

        var lastFetchDateTimeStr = PlayerPrefs.GetString("LastFetchDateTime");
        if (!lastFetchDateTimeStr.Equals(""))
            lastFetchDateTime = DateTime.Parse(lastFetchDateTimeStr);

        bool canFetch = CanFetchNewMissions(currDateTime, lastFetchDateTime);

        if (!canFetch)
        {
            try
            {
                //LoadingScreen.Instance.FakeOpen(1);
                if (_missionsList.Count == 0 || _missionsList == null)
                    _missionsList = await _missionLocalService.LoadAllMissions();

                SetMissionsRefreshTimer(currDateTime, lastFetchDateTime);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(FileNotFoundException))
                {
                    canFetch = true;
                }
            }
        }

        if (canFetch)
        {
            FetchNewRandomMissions();
            return;
        }

        _missionsCacheView.DisplayMissions(_missionsList);
    }

    public async void HandleMissionSelect(Mission mission)
    {
        LoadingScreen.Instance.FakeOpen(2);

        PlayerPrefs.SetInt("CurrentSelectedMissionId", mission.Id);

        if (await _missionLocalService.SaveMission(mission))
            GameManager.Instance.UpdateGameState(GameState.InitializingMission);
    }

    public void HandleMissionRefreshTimerEnded()
    {
        FetchNewRandomMissions();
    }

    private void SetMissionsRefreshTimer(DateTime currDateTime, DateTime lastFetchDateTime)
    {
        int timeRemainsInSec = (int)((_refreshTimeInHours * 3600) - Mathf.FloorToInt((float)(currDateTime - lastFetchDateTime).TotalSeconds));
        _missionsCacheView.DisplayTimeUntilMissionsRefresh(timeRemainsInSec);
    }

    public bool CanFetchNewMissions(DateTime currDateTime, DateTime lastFetchDateTime)
    {
        if (lastFetchDateTime == currDateTime)
            return true;

        // Check if 2 hours has passed since the last fetch time
        return (lastFetchDateTime - currDateTime).TotalSeconds >= _refreshTimeInHours;
    }

    private async void FetchNewRandomMissions()
    {
        if (isFetching)
            return;
        else
            isFetching = true;

        LoadingScreen.Instance.FakeOpen(1);

        _missionsCacheView.ClearMissionCards();
        //_missionsList = await _missionRemoteService.GetRandomRemoteMissions(_missionsCount);
        _missionsList = _missionLocalService.GetRandomMissions(_missionsCount);
        if (await _missionLocalService.SaveAllMissions(_missionsList))
        {
            PlayerPrefs.SetString("LastFetchDateTime", DateTime.Now.ToString());

            DateTime lastFetchDateTime = DateTime.Parse(PlayerPrefs.GetString("LastFetchDateTime"));
            SetMissionsRefreshTimer(DateTime.Now, lastFetchDateTime);

            _missionsCacheView.DisplayMissions(_missionsList);

            isFetching = false;
        }
    }
}
