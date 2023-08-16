using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMapService
{
    ILocalDataService _dataService = new JsonLocalDataService();

    public Mission GetLocalSavedMission()
    {
        return _dataService.LoadData<Mission>("/mission.json", true);
    }
}
