using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public Player Player { get; set; }
    PlayerPresenter _playerPresenter;

    [SerializeField] private MissionResultsView _missionResultsView;
    [SerializeField] private PlayerStatsView _playerStatsView;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _playerPresenter = new PlayerPresenter(this);

        HideAvatarFrame();
        HidePerformanceStats();
    }

    public async void SignInPlayer()
    {
        if (await _playerPresenter.SignInPlayer())
            GameManager.Instance.UpdateGameState(GameState.MainMenu);
        else
            ErrorScreen.Instance.Show("Sign in failed!");
    }

    public async void RegisterPlayer(string displayName, int gender)
    {
        if (await _playerPresenter.RegisterPlayer(displayName, gender))
            SignInPlayer();
        else
            ErrorScreen.Instance.Show("Register failed!");
    }

    public void SetPlayerMissionStats(int score, int reputation, int matches, int coins)
    {
        _playerPresenter.HandlePlayerMissionStatsSet(score, reputation, matches, coins);
    }

    public void DisplayMissionResults(bool isVictory)
    {
        _missionResultsView.gameObject.SetActive(true);

        int currScore = PlayerPrefs.GetInt("MissionScore");
        int currRep = PlayerPrefs.GetInt("MissionReputation");
        int currMatches = PlayerPrefs.GetInt("MissionMatches");
        MissionPerformance missionPerformance = new MissionPerformance()
        {
            TotalMissionScore = currScore,
            TotalReputation = currRep,
            TotalMatches = currMatches,
            IsVictory = isVictory,
            BonusReputation = 10,
            BonusCoins = 20,
        };

        Player.Reputation += missionPerformance.BonusReputation;
        Player.NetCoins += missionPerformance.BonusCoins;

        UpdatePlayerInformation();

        _missionResultsView.DisplayResultsScreen(missionPerformance);
    }

    public void UpdatePlayerInformation()
    {
        if (Player != null)
        {
            _playerStatsView.DisplayPlayerInformation(Player);
        }
    }

    public void ShowAvatarFrame() => _playerStatsView.ShowAvatarFrame();

    public void HideAvatarFrame() => _playerStatsView.HideAvatarFrame();

    public void ShowPerformanceStats() => _playerStatsView.ShowPerformanceStats();

    public void HidePerformanceStats() => _playerStatsView.HidePerformanceStats();
}
