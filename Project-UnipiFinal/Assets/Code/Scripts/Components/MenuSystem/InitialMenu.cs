using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InitialMenu : MenuManager
{
    public void Start()
    {
        OpenWelcomeMenu();
    }

    public void OpenWelcomeMenu()
    {
        ToggleMenu(Menu.Welcome);
    }

    public void OpenRegisterMenu()
    {
        ToggleMenu(Menu.Register);
    }
}
