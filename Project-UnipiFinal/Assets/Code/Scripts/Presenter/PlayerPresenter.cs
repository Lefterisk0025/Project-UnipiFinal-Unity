using System;
using System.Collections;
using System.Collections.Generic;
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
}
