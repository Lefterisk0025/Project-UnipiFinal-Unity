using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool PlayerOnMission = false;

    // Container for saving data
    [SerializeField] private CoreGameDataSO coreGameDataSO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        // When the game launches
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState state)
    {
        switch (state)
        {
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

    private void HandleOnMainMenuState()
    {
        if (!SceneManager.GetSceneByName("MainMenu").isLoaded)
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    private void HandleInitializingMissionState()
    {
        PlayerOnMission = true;

        MenuManager.Instance.ToggleMenu(Menu.MissionMap);
    }

    private void HandleOnMatchPointMatchState()
    {
        SceneManager.LoadScene("test", LoadSceneMode.Additive);

        SceneManager.UnloadSceneAsync("MainMenu");
    }

    private void HandleAbandoningMissionState()
    {
        PlayerOnMission = false;

        MenuManager.Instance.ToggleMenu(Menu.MainMenu);
    }
}
