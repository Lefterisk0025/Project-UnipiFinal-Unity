using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ObjectiveService
{
    private const string localObjectivesListFileName = "/objectives.json";

    ILocalDataService _dataService;

    public ObjectiveService()
    {
        _dataService = new JsonLocalDataService();
    }

    public async Task<bool> SaveLocalObjectivesListData(List<Objective> objectivesList)
    {
        if (await _dataService.SaveData(localObjectivesListFileName, objectivesList, true))
            return true;

        return false;
    }

    public async Task<List<Objective>> LoadLocalObjectivesList()
    {
        return await _dataService.LoadData<List<Objective>>(localObjectivesListFileName, true);
    }

    public async Task<bool> DeleteObjectivesList()
    {
        if (await _dataService.DeleteData(localObjectivesListFileName))
            return true;

        return false;
    }
}
