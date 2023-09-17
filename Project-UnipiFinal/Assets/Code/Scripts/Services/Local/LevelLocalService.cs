using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelLocalService
{
    private const string localLevelsFileName = "/levels.json";

    ILocalDataService _dataService;

    public LevelLocalService()
    {
        _dataService = new JsonLocalDataService();
    }

    public async Task<bool> SaveAllLevels(List<Level> levelsList)
    {
        return await _dataService.SaveData(localLevelsFileName, levelsList, true);
    }

    public async Task<List<Level>> LoadAllLevels()
    {
        return await _dataService.LoadData<List<Level>>(localLevelsFileName, true);
    }

    public async Task<bool> DeleteAllLevels()
    {
        return await _dataService.DeleteData(localLevelsFileName);
    }
}
