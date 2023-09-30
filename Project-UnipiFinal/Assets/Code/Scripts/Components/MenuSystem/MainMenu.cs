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
        PlayerManager.Instance.UpdatePlayerInformation();
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
            LoadingScreen.Instance.FakeOpen(1);
            PlayerManager.Instance.HidePerformanceStats();
            ToggleMenu(Menu.MissionMap);
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
