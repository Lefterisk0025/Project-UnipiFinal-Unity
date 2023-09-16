using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MissionLocalService
{
    private const string localMissionFileName = "/mission.json";
    private const string localMissionsCacheFileName = "/missions_chache.json";

    ILocalDataService _dataService;

    public MissionLocalService()
    {
        _dataService = new JsonLocalDataService();
    }

    public async Task<bool> SaveMission(Mission mission)
    {
        return await _dataService.SaveData(localMissionFileName, mission, true);
    }

    public async Task<Mission> LoadMission()
    {
        return await _dataService.LoadData<Mission>(localMissionFileName, true);
    }

    public async Task<bool> DeleteMission()
    {
        return await _dataService.DeleteData(localMissionFileName);
    }

    public async Task<bool> SaveAllMissions(List<Mission> missionsList)
    {
        return await _dataService.SaveData(localMissionsCacheFileName, missionsList, true);
    }

    public async Task<List<Mission>> LoadAllMissions()
    {
        return await _dataService.LoadData<List<Mission>>(localMissionsCacheFileName, true);
    }

    public async Task<bool> DeleteAllMissions()
    {
        return await _dataService.DeleteData(localMissionsCacheFileName);
    }
}
