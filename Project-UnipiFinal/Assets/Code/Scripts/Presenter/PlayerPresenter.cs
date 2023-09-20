using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerPresenter
{
    PlayerService _playerService;
    PlayerManager _playerManager;

    AuthService _authService;

    public PlayerPresenter(PlayerManager playerManager)
    {
        _playerManager = playerManager;

        _playerService = new PlayerService();
        _authService = new AuthService();
    }

    public string GetRemoteAuthUserId()
    {
        return _playerService.GetRemoteAuthUserId();
    }

    public async Task<Player> GetRemotePlayerByUserId(string userId)
    {
        return await _playerService.GetRemotePlayerByUserId(userId);
    }

    public async Task<bool> CreateRemotePlayer(string userId, string displayName)
    {
        return await _playerService.CreateRemotePlayer(userId, displayName);
    }

    public async Task<bool> SignInPlayer()
    {
        try
        {
            if (_authService.AuthGooglePlayGames())
            {
                string userId = GetRemoteAuthUserId();
                var player = await GetRemotePlayerByUserId(userId);

                if (player == null)
                    GameManager.Instance.UpdateGameState(GameState.OnAuthMenu);

                PlayerManager.Instance.Player = player;

                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public async Task<bool> RegisterPlayer(string displayName)
    {
        try
        {
            if (_authService.AuthGooglePlayGames())
            {
                string userId = GetRemoteAuthUserId();

                if (await CreateRemotePlayer(userId, displayName))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public async Task<MissionPerformance> GetPlayerMissionPerformance()
    {
        return await _playerService.LoadLocalPlayerMissionPerformanceDataAsync();
    }

    public async Task<bool> UpdatePlayerMissionPerformance()
    {
        // Save the local data
        MissionPerformance missionPerformance = null;
        try
        {
            missionPerformance = await _playerService.LoadLocalPlayerMissionPerformanceDataAsync();
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.Message);

            missionPerformance = new MissionPerformance();
        }

        //missionPerformance.TotalMissionScore += matchResults.TotalScore;
        //missionPerformance.TotalReputation += matchResults.ReputationEarned;

        await _playerService.SaveLocalPlayerMissionPerformanceDataAsync(missionPerformance);

        return true;

        // Update player data to the server the match results
        // if (await _playerService.UpdateRemoteProgressionStatsOfPlayer(matchResults, PlayerManager.Instance.Player.UserId))
        //     return true;
        // else
        //     return false;
    }

    public async Task<bool> DeleteMissionPerformace()
    {
        return await _playerService.DeleteLocalPlayerMissionPerformanceDataAsync();
    }
}
