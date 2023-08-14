using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool PlayerOnMission { get => coreGameDataSO.PlayerOnMission; private set => coreGameDataSO.PlayerOnMission = value; }

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
        }
    }

    private void HandleOnMainMenuState()
    {
        if (!SceneManager.GetSceneByName("MainMenu").isLoaded)
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    private void HandleInitializingMissionState()
    {
        coreGameDataSO.PlayerOnMission = true;
    }

    private void HandleOnMatchPointMatchState()
    {
        SceneManager.LoadScene("test", LoadSceneMode.Additive);

        SceneManager.UnloadSceneAsync("MainMenu");
    }
}
