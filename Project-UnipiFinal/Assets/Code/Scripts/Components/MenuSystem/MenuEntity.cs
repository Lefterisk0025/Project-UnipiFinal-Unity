using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEntity : MonoBehaviour
{
    public Menu MenuType;

    private void OnEnable()
    {
        if (MenuType == Menu.MainMenu)
            AudioManager.Instance.PlayMainMenuMusic();
    }
}
