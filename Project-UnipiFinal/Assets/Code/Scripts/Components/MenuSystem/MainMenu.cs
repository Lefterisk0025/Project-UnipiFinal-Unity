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
        ToggleMenu(_menusDict[Menus.MAIN_MENU]);
    }

    public void OpenGameModesMenu()
    {
        ToggleMenu(_menusDict[Menus.GAMEMODES]);
    }

    public void OpenStoreMenu()
    {
        ToggleMenu(_menusDict[Menus.STORE]);
    }

    public void OpenStoreItemMenu()
    {
        ToggleMenu(_menusDict[Menus.STORE_ITEM]);
    }

    public void StartMatchPointMatch()
    {
    }

    public void StartTimeAttackMatch()
    {
    }
}
