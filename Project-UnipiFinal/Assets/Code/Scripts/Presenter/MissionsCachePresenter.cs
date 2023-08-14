using System.Collections;
using System.Collections.Generic;
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

    public List<Mission> GetRandomMissions(int count)
    {
        return _missionsCacheService.GetRandomMissions(count);
    }
}
