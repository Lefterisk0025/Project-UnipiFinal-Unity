using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Initialization);
    }

    public void UpdateGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Initialization:
                HandleInitializationState();
                break;
            case GameState.OnAuthMenu:
                HandleOnAuthMenuState();
                break;
            case GameState.MainMenu:
                HandleOnMainMenuState();
                break;
            case GameState.InitializingMission:
                HandleInitializingMissionState();
                break;
            case GameState.AbandoningMission:
                HandleAbandoningMissionState();
                break;
        }
    }

    private void HandleInitializationState()
    {
        // Logo screen

        PlayerManager.Instance.SignInPlayer();
    }

    private void HandleOnAuthMenuState()
    {
        LoadingScreen.Instance.Open();

        if (!SceneManager.GetSceneByName("AuthMenu").isLoaded)
            SceneManager.LoadScene("AuthMenu", LoadSceneMode.Additive);

        LoadingScreen.Instance.Close();
    }

    private void HandleOnMainMenuState()
    {
        LoadingScreen.Instance.FakeOpen(2);

        CustomSceneManager.Instance.SwitchScene("AuthMenu", "MainMenu");
    }

    private void HandleInitializingMissionState()
    {
        PlayerPrefs.SetInt("OnMission", 1);

        MenuManager.Instance.ToggleMenu(Menu.MissionMap);
    }

    private void HandleOnMatchPointMatchState()
    {
        SceneManager.LoadScene("test", LoadSceneMode.Additive);

        SceneManager.UnloadSceneAsync("MainMenu");
    }

    private void HandleAbandoningMissionState()
    {
        PlayerPrefs.SetInt("OnMission", 0);

        MenuManager.Instance.ToggleMenu(Menu.MainMenu);
    }
}