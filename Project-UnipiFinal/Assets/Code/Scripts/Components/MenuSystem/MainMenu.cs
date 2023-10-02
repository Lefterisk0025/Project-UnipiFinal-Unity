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
        PlayerManager.Instance.UpdateDisplayOfPlayerInformation();
        PlayerManager.Instance.ShowAvatarFrame();
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

    public void OpenGame()
    {
        ToggleMenu(Menu.Game);
    }
}
