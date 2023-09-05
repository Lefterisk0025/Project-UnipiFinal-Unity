using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuthenticationView : MonoBehaviour
{
    [SerializeField] private TMP_InputField _displayNameInputField;

    public void InvokeRegisterPlayer()
    {
        var nameText = _displayNameInputField.text;

        if (nameText != null)
        {
            PlayerManager.Instance.RegisterPlayer(nameText);

            GameManager.Instance.UpdateGameState(GameState.MainMenu);
        }
    }
}
