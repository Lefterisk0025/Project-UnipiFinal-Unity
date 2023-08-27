using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuManager
{
    private void Start()
    {
        OpenLoginMenu();
    }

    public void OpenLoginMenu()
    {
        ToggleMenu(Menu.Login);
    }

    public void OpenMainMenu()
    {
        ToggleMenu(Menu.MainMenu);
    }

    public void OpenMissionsMenu()
    {
        if (!GameManager.Instance.PlayerOnMission)
            ToggleMenu(Menu.Missions);
        else
            ToggleMenu(Menu.MissionMap);
    }

    public void OpenStoreMenu()
    {
        ToggleMenu(Menu.Store);
    }

    public void OpenStoreItemMenu()
    {
        ToggleMenu(Menu.StoreItem);
    }
}
