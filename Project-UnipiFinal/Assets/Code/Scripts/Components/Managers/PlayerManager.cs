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

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _playerPresenter = new PlayerPresenter(this);
    }

    public async void SignInPlayer()
    {
        if (await _playerPresenter.SignInPlayer())
            GameManager.Instance.UpdateGameState(GameState.MainMenu);
        else
            ErrorScreen.Instance.Show("Sign in failed!");
    }

    public async void RegisterPlayer(string displayName)
    {
        if (await _playerPresenter.RegisterPlayer(displayName))
            SignInPlayer();
        else
            ErrorScreen.Instance.Show("Register failed!");
    }

    public async void UpdatePlayerMissionPerformance(MatchResults matchResults)
    {
        if (await _playerPresenter.UpdatePlayerMissionPerformance(matchResults))
            Debug.Log("<color=green>Player performance updated succefully!</color>");
        else
            Debug.Log("<color=red>An error occured while updating player performance.</color>");
    }

    public async void ClearPlayerMissionPerformace()
    {
        if (await _playerPresenter.DeleteMissionPerformace())
            Debug.Log("<color=green>Player performance cleared succefully!</color>");
        else
            Debug.Log("<color=red>An error occured while clearing player performance.</color>");
    }

    public async void OpenMissionResultsUI()
    {
        MissionPerformance missionPerformance = await _playerPresenter.GetPlayerMissionPerformance();

        _missionResultsView.SetResultsScreen(missionPerformance);
    }

    public async void CloseMissionResultsUI()
    {
        await _playerPresenter.DeleteMissionPerformace();

        _missionResultsView.ClosePanelAndResetUI();
    }
}
