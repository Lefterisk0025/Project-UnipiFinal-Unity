using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MissionsCachePresenter
{
    MissionService _missionService;
    MissionsCacheView _missionsCacheView;
    List<Mission> _missionsCache;

    public MissionsCachePresenter(MissionsCacheView missionsCacheView)
    {
        _missionsCacheView = missionsCacheView;
        _missionService = new MissionService();
        _missionsCache = new List<Mission>();
    }

    public async Task<List<Mission>> GetLocalMissionsCacheData()
    {
        return await _missionService.GetLocalMissionsCacheData();
    }

    public async void SaveLocalMissionsCacheData(List<Mission> missionsCache)
    {
        if (await _missionService.SaveLocalMissionsCacheData(missionsCache))
        {
            Debug.Log("Data saved successfully!");
        }
    }

    public async Task<bool> UpdateLocalMissionsCacheData(List<Mission> missionsCache)
    {
        if (await _missionService.SaveLocalMissionsCacheData(missionsCache))
            return true;

        return false;
    }

    public async Task<bool> DeleteLocalMissionsCacheData()
    {
        if (await _missionService.DeleteLocalMissionCacheData())
            return true;

        return false;
    }

    // FOR TESTING
    public List<Mission> GetRandomLocalMissions(int count)
    {
        return _missionService.GetRandomLocalMissions(count);
    }

    public async Task<List<Mission>> GetRandomRemoteMissions(int count)
    {
        return await _missionService.GetRandomRemoteMissions(count);
    }

    public bool CanFetchNewMissions(DateTime lastFetchDateTime, DateTime currDateTime)
    {
        if (lastFetchDateTime == currDateTime)
            return true;

        // Check if 2 hours has passed since the last fetch time
        return (lastFetchDateTime - currDateTime).TotalHours >= 2.0;
    }
}
