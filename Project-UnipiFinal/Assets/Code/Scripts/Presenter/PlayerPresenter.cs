using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerPresenter
{
    PlayerLocalService _playerLocalService;
    PlayerRemoteService _playerRemoteService;
    PlayerManager _playerManager;

    AuthService _authService;

    public PlayerPresenter(PlayerManager playerManager)
    {
        _playerManager = playerManager;

        _playerLocalService = new PlayerLocalService();
        _playerRemoteService = new PlayerRemoteService();
        _authService = new AuthService();
    }

    public async Task<bool> SignInPlayer()
    {
        try
        {
            if (_authService.AuthGooglePlayGames())
            {
                string userId = _playerRemoteService.GetUserIdOfAuthUser();
                var player = await _playerRemoteService.GetPlayerByUserId(userId);

                if (player == null)
                    GameManager.Instance.UpdateGameState(GameState.OnAuthMenu);
                else
                {
                    PlayerManager.Instance.Player = player;
                    return true;
                }
            }

            return false;
        }
        catch (Exception e)
        {
            ErrorScreen.Instance.Show(e.Message);
            return false;
        }
    }

    public async Task<bool> RegisterPlayer(string displayName, int gender)
    {
        try
        {
            if (_authService.AuthGooglePlayGames())
            {
                string userId = _playerRemoteService.GetUserIdOfAuthUser();

                if (await _playerRemoteService.CreatePlayer(userId, displayName, gender))
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

    public void HandlePlayerMissionStatsSet(int score, int reputation, int matches, int coins)
    {
        int currScore = PlayerPrefs.GetInt("MissionScore"); // They are being initialized upon Level creation in MissionMapPresenter
        int currRep = PlayerPrefs.GetInt("MissionReputation");
        int currMartches = PlayerPrefs.GetInt("MissionMatches");

        PlayerPrefs.SetInt("MissionScore", currScore + score);
        PlayerPrefs.SetInt("MissionReputation", currRep + reputation);
        PlayerPrefs.SetInt("MissionMatches", currMartches + matches);

        PlayerManager.Instance.Player.Reputation += reputation;
        PlayerManager.Instance.Player.NetCoins += coins;

        PlayerManager.Instance.UpdatePlayerInformation();

        //await _playerRemoteService.UpdatePlayer(PlayerManager.Instance.Player);
    }
}
