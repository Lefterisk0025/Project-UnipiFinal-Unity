using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public UnityEvent OnMissionTerminate;

    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        DisableMainCamera();
    }

    private void Start()
    {
        UpdateGameState(GameState.Initialization);
    }

    public void DisableMainCamera() => mainCamera.gameObject.SetActive(false);
    public void EnableMainCamera() => mainCamera.gameObject.SetActive(true);


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
            case GameState.Playing:
                HandlePlayingState();
                break;
            case GameState.FinishingLevel:
                HandleFinishingLevelState();
                break;
            case GameState.TerminatingMission:
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

    private void HandleAbandoningMissionState()
    {
        PlayerPrefs.SetInt("OnMission", 0); // Its also being set in LevelView

        PlayerManager.Instance.ShowAvatar();
        PlayerManager.Instance.ShowPerformanceStats();

        MenuManager.Instance.ToggleMenu(Menu.MainMenu);
    }

    private void HandlePlayingState()
    {
        LoadingScreen.Instance.FakeOpen(1);

        AudioManager.Instance.StopAllMusics();

        MenuManager.Instance.ToggleMenu(Menu.Game);
    }

    private void HandleFinishingLevelState()
    {
        AudioManager.Instance.StopAllEffects();

        MenuManager.Instance.ToggleMenu(Menu.MissionMap);
    }
}
