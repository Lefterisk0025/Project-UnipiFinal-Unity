using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissionsCachePresenter
{
    MissionsCacheView _missionsCacheView;
    MissionsCacheService _missionsCacheService;

    public MissionsCachePresenter(MissionsCacheView missionsCacheView)
    {
        _missionsCacheView = missionsCacheView;

        _missionsCacheService = new MissionsCacheService();
    }

    public List<Mission> GetNewRandomMissions(int count)
    {
        return _missionsCacheService.GetNewRandomMissions(count);
    }

    public List<Mission> GetCurrentMissions()
    {
        return null;
    }

    public void SaveMissionDataLocal(Mission mission)
    {
        if (_missionsCacheService.SaveMissionDataLocal(mission))
        {
            Debug.Log("Data saved successfully!");
        }
    }
}

