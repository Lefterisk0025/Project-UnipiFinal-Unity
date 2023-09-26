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
    MissionLocalService _missionLocalService;
    MissionRemoteService _missionRemoteService;
    MissionsCacheView _missionsCacheView;
    List<Mission> _missionsList;

    //bool canFetch = false;

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

        SetMissionsRefreshTimer(currDateTime, lastFetchDateTime);

        bool canFetch = CanFetchNewMissions(currDateTime, lastFetchDateTime);

        if (!canFetch)
        {
            // if (_missionsList != null && _missionsList.Count > 0)
            // {
            //     _missionsCacheView.DisplayMissions(_missionsList);
            //     return;
            // }

            try
            {
                Debug.Log("<color=yellow>Loading Missions</color>");
                //LoadingScreen.Instance.FakeOpen(1);
                _missionsList = await _missionLocalService.LoadAllMissions();
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

        await _missionLocalService.SaveMission(mission);

        GameManager.Instance.UpdateGameState(GameState.InitializingMission);
    }

    public void HandleMissionRefreshTimerEnded()
    {
        FetchNewRandomMissions();
    }

    private void SetMissionsRefreshTimer(DateTime currDateTime, DateTime lastFetchDateTime)
    {
        int timeRemainsInSec = 7200 - Mathf.FloorToInt((float)(currDateTime - lastFetchDateTime).TotalSeconds);
        _missionsCacheView.DisplayTimeUntilMissionsRefresh(timeRemainsInSec);
    }

    public bool CanFetchNewMissions(DateTime currDateTime, DateTime lastFetchDateTime)
    {
        if (lastFetchDateTime == currDateTime)
            return true;

        // Check if 2 hours has passed since the last fetch time
        return (lastFetchDateTime - currDateTime).TotalHours >= 2.0f;
    }

    private async void FetchNewRandomMissions()
    {
        Debug.Log("<color=yellow>Fetching Missions</color>");
        LoadingScreen.Instance.FakeOpen(1);

        _missionsCacheView.ClearMissionCards();
        //_missionsList = await _missionRemoteService.GetRandomRemoteMissions(_missionsCount);
        _missionsList = _missionLocalService.GetRandomMissions(_missionsCount);
        await _missionLocalService.SaveAllMissions(_missionsList);

        PlayerPrefs.SetString("LastFetchDateTime", DateTime.Now.ToString());

        DateTime lastFetchDateTime = DateTime.Parse(PlayerPrefs.GetString("LastFetchDateTime"));
        SetMissionsRefreshTimer(DateTime.Now, lastFetchDateTime);

        _missionsCacheView.DisplayMissions(_missionsList);
    }
}
