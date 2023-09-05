using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _displayNameInputField;

    public void InvokeCreateAccount()
    {
        if (_displayNameInputField.text == "")
            return;

        //PlayerManager.Instance.InvokeCreatePlayerAccount(_displayNameInputField.text);
    }
}
