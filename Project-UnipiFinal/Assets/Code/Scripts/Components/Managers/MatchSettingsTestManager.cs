using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSettingsTestManager : MonoBehaviour
{
    [Header("General UI")]
    [SerializeField] private TMP_Dropdown _gameModesDropdown;
    [SerializeField] private GameObject _timeAttackMenu;
    [SerializeField] private GameObject _matchPointMenu;
    [SerializeField] private TMP_InputField _gridHeightIF;

    [Header("Time Attack")]
    [SerializeField] private TMP_InputField _findMatchTimeIF;
    [SerializeField] private TMP_InputField _numberOfMatchesPerTimeIF;

    [Header("Match Point")]
    [SerializeField] private TMP_InputField _pointsGoalIF;
    [SerializeField] private TMP_InputField _pointsPerMatchIF;
    [SerializeField] private TMP_InputField _totalTimeIF;

    private GameObject _currSelectedMenu;

    private void Awake()
    {
        _timeAttackMenu.SetActive(false);
        _matchPointMenu.SetActive(false);
    }

    public void ToggleGameModesDropDown()
    {
        switch (_gameModesDropdown.value)
        {
            case 0:
                _currSelectedMenu.SetActive(false);
                break;
            case 1:
                ToggleGameModeMenu(1); // Time Attack
                break;
            case 2:
                ToggleGameModeMenu(2); // Match Point
                break;
        }
    }

    private void ToggleGameModeMenu(int index)
    {
        if (_currSelectedMenu != null && _currSelectedMenu.activeSelf)
            _currSelectedMenu.SetActive(false);

        if (index == 1)
            _currSelectedMenu = _timeAttackMenu;
        else
            _currSelectedMenu = _matchPointMenu;

        _currSelectedMenu.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public int GetGameMode()
    {
        return _gameModesDropdown.value;
    }

    public int GetHeight()
    {
        return int.Parse(_gridHeightIF.text);
    }

    public int GetFindMatchTime()
    {
        return int.Parse(_findMatchTimeIF.text);
    }

    public int GetNumberOfMatchesPerTime()
    {
        return int.Parse(_numberOfMatchesPerTimeIF.text);
    }

    public int GetPointsGoal()
    {
        return int.Parse(_pointsGoalIF.text);
    }

    public int GetPointsPerMatchGoal()
    {
        return int.Parse(_pointsPerMatchIF.text);
    }

    public int GetTotalTime()
    {
        return int.Parse(_totalTimeIF.text);
    }
}
