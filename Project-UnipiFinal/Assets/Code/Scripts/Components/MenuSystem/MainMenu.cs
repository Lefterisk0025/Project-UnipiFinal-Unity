using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MenuManager
{
    [SerializeField] private TextMeshProUGUI _welcomePlayer;

    private void Start()
    {
        OpenMainMenu();
    }

    private void OnEnable()
    {
        if (PlayerManager.Instance.Player != null)
            _welcomePlayer.text = "Username: " + PlayerManager.Instance.Player.DisplayName + "\nUserId: " + PlayerManager.Instance.Player.UserId;
        else
            _welcomePlayer.text = "There is no player...";
    }

    public void OpenMainMenu()
    {
        ToggleMenu(Menu.MainMenu);
    }

    public void OpenMissionsMenu()
    {
        int onMission = PlayerPrefs.GetInt("OnMission");
        if (onMission == 0)
            ToggleMenu(Menu.Missions);
        else
        {
            LoadingScreen.Instance.FakeOpen(1);
            ToggleMenu(Menu.MissionMap);
        }
    }

    public void OpenStoreMenu()
    {
        ToggleMenu(Menu.Store);
    }

    public void OpenStoreItemMenu()
    {
        ToggleMenu(Menu.StoreItem);
    }

    public void OpenGame()
    {
        ToggleMenu(Menu.Game);
    }
}
