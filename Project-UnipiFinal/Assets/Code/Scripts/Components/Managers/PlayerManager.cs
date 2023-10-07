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
            _playerPresenter.LoadLocalPlayer();
    }

    public async void RegisterPlayer(string displayName, int gender)
    {
        if (await _playerPresenter.RegisterPlayer(displayName, gender))
            SignInPlayer();
        else
            _playerPresenter.CreateAndLoadLocalPlayer(displayName, gender);
    }

    public void IncrementPerformanceStats(LevelPerformance performance)
    {
        _playerPresenter.HandlePerformanceStatsIncrement(performance);
    }

    public void SetMissionResults(bool isVictory)
    {
        _playerPresenter.HandleMissionResultsSet(isVictory);
    }

    public void DisplayMissionResults(MissionPerformance missionPerformance)
    {
        _missionResultsView.gameObject.SetActive(true);
        _missionResultsView.DisplayResultsScreen(missionPerformance);
    }

    public void UpdateDisplayOfPlayerInformation()
    {
        if (Player != null)
            _playerStatsView.DisplayPlayerInformation(Player);
    }

    public void ShowAvatarFrame() => _playerStatsView.ShowAvatarFrame();

    public void HideAvatarFrame() => _playerStatsView.HideAvatarFrame();

    public void ShowPerformanceStats() => _playerStatsView.ShowPerformanceStats();

    public void HidePerformanceStats() => _playerStatsView.HidePerformanceStats();

    public void BuyItem(int price)
    {
        _playerPresenter.HandleBuyItem(price);
    }
}
