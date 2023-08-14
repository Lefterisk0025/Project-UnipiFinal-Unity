using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuManager
{
    void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        ToggleMenu(_menusDict[Menus.MainMenu]);
    }

    public void OpenMissionsMenu()
    {
        if (!GameManager.Instance.PlayerOnMission)
            ToggleMenu(_menusDict[Menus.Missions]);
        else
            ToggleMenu(_menusDict[Menus.MissionMap]);

    }

    public void OpenStoreMenu()
    {
        ToggleMenu(_menusDict[Menus.Store]);
    }

    public void OpenStoreItemMenu()
    {
        ToggleMenu(_menusDict[Menus.StoreItem]);
    }

    public void StartMatchPointMatch()
    {
    }

    public void StartTimeAttackMatch()
    {
    }
}
