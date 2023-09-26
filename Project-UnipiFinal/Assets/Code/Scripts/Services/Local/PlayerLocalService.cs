using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerLocalService
{
    private const string localPlayerFileName = "/player.json";

    ILocalDataService _dataService;

    public PlayerLocalService()
    {
        _dataService = new JsonLocalDataService();
    }

    public async Task<bool> SavePlayerDataAsync(Player player)
    {
        return await _dataService.SaveData(localPlayerFileName, player, true);
    }

    public async Task<Player> LoadPlayerDataAsync()
    {
        return await _dataService.LoadData<Player>(localPlayerFileName, true);
    }

    public async Task<bool> DeletePlayerDataAsync()
    {
        return await _dataService.DeleteData(localPlayerFileName);
    }
}
