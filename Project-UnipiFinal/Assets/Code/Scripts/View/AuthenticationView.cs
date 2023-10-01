using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

public class AuthenticationView : MonoBehaviour
{
    [SerializeField] private TMP_InputField _displayNameInputField;

    int _genderIndex = -1;

    public void InvokeRegisterPlayer()
    {
        var nameText = _displayNameInputField.text;

        // Data validation
        if (ValidateData(nameText, _genderIndex))
        {
            PlayerManager.Instance.RegisterPlayer(nameText, _genderIndex);
            GameManager.Instance.UpdateGameState(GameState.MainMenu);
        }
        else
        {
            AlertWindow.Instance.ShowMessageAlert("Register failed!", "Invalid display name or gender.");
        }
    }

    public void OnSelectMaleGender() => _genderIndex = 0;

    public void OnSelectFemaleGender() => _genderIndex = 1;

    public void OnSelectOtherGender() => _genderIndex = 2;

    private bool ValidateData(string displayName, int gender)
    {
        if (displayName.Equals("") || displayName == null)
            return false;

        if (displayName.Length < 3 || displayName.Length > 15)
            return false;

        if (!Regex.IsMatch(displayName, "^[A-Za-z0-9]*$"))
            return false;

        if (gender == -1)
            return false;

        return true;
    }

    public void OnHelpButtonClicked()
    {
        AlertWindow.Instance.ShowMessageAlert("Help", "Name must contains only letters and numbers and must be of lenght 3 to 15");
    }
}
