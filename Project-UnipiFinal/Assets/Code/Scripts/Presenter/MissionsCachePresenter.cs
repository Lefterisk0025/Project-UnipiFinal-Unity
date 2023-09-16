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

        _missionsCacheView.OnViewInitialized.AddListener(HandleViewInitialized);
    }

    public async void HandleViewInitialized(int count, bool canFetch)
    {
        List<Mission> tempMissions = null;
        if (!canFetch)
        {
            try
            {
                tempMissions = await _missionLocalService.LoadAllMissions();
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
            LoadingScreen.Instance.FakeOpen(1);

            _missionsCacheView.ClearMissionCards();
            tempMissions = await _missionRemoteService.GetRandomRemoteMissions(_missionsCount);
            await _missionLocalService.SaveAllMissions(tempMissions);
            PlayerPrefs.SetString("LastFetchDateTime", DateTime.Now.ToString());
        }

        _missionsCacheView.DisplayMissions(tempMissions);
    }

    public bool CanFetchNewMissions(DateTime lastFetchDateTime, DateTime currDateTime)
    {
        if (lastFetchDateTime == currDateTime)
            return true;

        // Check if 2 hours has passed since the last fetch time
        return (lastFetchDateTime - currDateTime).TotalHours >= 2.0;
    }
}
