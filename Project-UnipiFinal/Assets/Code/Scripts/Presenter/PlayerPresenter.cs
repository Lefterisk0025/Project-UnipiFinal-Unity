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

    public async Task<int> SignInPlayer()
    {
        try
        {
            if (_authService.AuthGooglePlayGames())
            {
                string userId = _playerRemoteService.GetUserIdOfAuthUser();
                var playerRemote = await _playerRemoteService.GetPlayerByUserId(userId);

                if (playerRemote == null)
                {
                    GameManager.Instance.UpdateGameState(GameState.OnAuthMenu);
                    return 2;
                }
                else
                {
                    PlayerManager.Instance.Player = playerRemote;
                    return 0;
                }
            }
            else
                return 1;
        }
        catch (Exception e)
        {
            ErrorScreen.Instance.Show(e.Message);
            return 1;
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

    public async void HandlePerformanceStatsIncrement(LevelPerformance performance)
    {
        int currScore = PlayerPrefs.GetInt("MissionScore"); // They are being initialized upon Level creation in MissionMapPresenter
        int currRep = PlayerPrefs.GetInt("MissionReputation");
        int currMartches = PlayerPrefs.GetInt("MissionMatches");

        PlayerPrefs.SetInt("MissionScore", currScore + performance.TotalScore);
        PlayerPrefs.SetInt("MissionReputation", currRep + performance.ReputationEarned);
        PlayerPrefs.SetInt("MissionMatches", currMartches + performance.TotalMatches);

        PlayerManager.Instance.Player.Reputation += performance.ReputationEarned;
        PlayerManager.Instance.Player.NetCoins += performance.CoinsEarned;

        await _playerRemoteService.UpdatePlayer(PlayerManager.Instance.Player);

        PlayerManager.Instance.UpdateDisplayOfPlayerInformation();
    }

    public async void HandleMissionResultsSet(bool isVictory)
    {
        int currScore = PlayerPrefs.GetInt("MissionScore");
        int currRep = PlayerPrefs.GetInt("MissionReputation");
        int currMatches = PlayerPrefs.GetInt("MissionMatches");

        MissionPerformance missionPerformance = new MissionPerformance()
        {
            TotalMissionScore = currScore,
            TotalReputation = currRep,
            TotalMatches = currMatches,
            IsVictory = isVictory,
            BonusReputation = (int)Mathf.Ceil((currScore + currMatches) / 20),
            BonusCoins = (int)Mathf.Ceil(currScore / 20),
        };

        PlayerManager.Instance.Player.Reputation += missionPerformance.BonusReputation;
        PlayerManager.Instance.Player.NetCoins += missionPerformance.BonusCoins;

        PlayerManager.Instance.DisplayMissionResults(missionPerformance);

        await _playerRemoteService.UpdatePlayer(PlayerManager.Instance.Player);

        PlayerManager.Instance.UpdateDisplayOfPlayerInformation();
    }

    public async void HandleBuyItem(int price)
    {
        PlayerManager.Instance.Player.NetCoins -= price;
        PlayerManager.Instance.UpdateDisplayOfPlayerInformation();
        await _playerRemoteService.UpdatePlayer(PlayerManager.Instance.Player);
    }
}
