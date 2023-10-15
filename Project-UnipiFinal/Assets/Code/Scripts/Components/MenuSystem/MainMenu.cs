using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MenuManager
{
    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        AudioManager.Instance.PlayMainMenuMusic();

        PlayerManager.Instance.UpdateDisplayOfPlayerInformation();
        PlayerManager.Instance.ShowAvatar();
        PlayerManager.Instance.ShowPerformanceStats();

        ToggleMenu(Menu.MainMenu);
    }

    public void OpenMissionsMenu()
    {
        int onMission = PlayerPrefs.GetInt("OnMission");
        if (onMission == 0)
        {
            PlayerManager.Instance.ShowPerformanceStats();
            ToggleMenu(Menu.Missions);
        }
        else
        {
            PlayerManager.Instance.HidePerformanceStats();
            Debug.Log("Opening Mission Map Menu");
            GameManager.Instance.UpdateGameState(GameState.InitializingMission);
        }
    }

    public void OpenStoreMenu()
    {
        ToggleMenu(Menu.Store);
    }

    public void OpenTutorial()
    {
        PlayerManager.Instance.HideAvatar();
        PlayerManager.Instance.HidePerformanceStats();

        ToggleMenu(Menu.Tutorial);
    }

    public void OpenSettings()
    {
        PlayerManager.Instance.HideAvatar();
        PlayerManager.Instance.HidePerformanceStats();

        ToggleMenu(Menu.Settings);
    }

    public void ExitGame()
    {
        AlertWindow.Instance.ShowYesNoAlert("Confirmation", "Are you sure you want to exit?", () =>
        {
            Application.Quit();
        }, () => { });
    }
}
