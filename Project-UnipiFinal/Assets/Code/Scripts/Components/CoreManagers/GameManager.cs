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
        UpdateGameState(GameState.ON_MAIN_MENU);
    }

    public void UpdateGameState(GameState state)
    {
        switch (state)
        {
            case GameState.ON_MAIN_MENU:
                HandleOnMainMenuState();
                break;
            case GameState.ON_HACKRUN:
                break;
            case GameState.ON_MATCH:
                break;
        }
    }

    private void HandleOnMainMenuState()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    private void HandleOnMatchPointMatchState()
    {
        SceneManager.LoadScene("test", LoadSceneMode.Additive);

        SceneManager.UnloadSceneAsync("MainMenu");
    }

    private void HandleOnTimeAttackMatchState()
    {
        SceneManager.LoadScene("test", LoadSceneMode.Additive);

        SceneManager.UnloadSceneAsync("MainMenu");
    }
}
