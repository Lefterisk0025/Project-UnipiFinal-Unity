using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMapPresenter
{
    MissionMapView _missionMapView;
    MissionMapService _missionMapService;

    public MissionMapPresenter(MissionMapView missionMapView)
    {
        _missionMapView = missionMapView;

        _missionMapService = new MissionMapService();
    }

    public Mission GetLocalSavedMission()
    {
        return _missionMapService.GetLocalSavedMission();
    }

    public List<MapNode> CreateMapGraph()
    {
        return null;
    }
}
