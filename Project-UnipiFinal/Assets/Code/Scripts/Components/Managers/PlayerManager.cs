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

    public void SetPlayerMissionStats(int score, int reputation)
    {
        int currScore = PlayerPrefs.GetInt("MissionScore");
        int currRep = PlayerPrefs.GetInt("MissionReputation");

        PlayerPrefs.SetInt("MissionScore", currScore + score);
        PlayerPrefs.SetInt("MissionReputation", currRep + reputation);
    }

    public void DisplayMissionResults(bool isVictory)
    {
        _missionResultsView.gameObject.SetActive(true);

        int currScore = PlayerPrefs.GetInt("MissionScore");
        int currRep = PlayerPrefs.GetInt("MissionReputation");
        MissionPerformance missionPerformance = new MissionPerformance()
        {
            TotalMissionScore = currScore,
            TotalReputation = currRep,
            IsVictory = isVictory,
        };
        _missionResultsView.DisplayResultsScreen(missionPerformance);
    }
}
