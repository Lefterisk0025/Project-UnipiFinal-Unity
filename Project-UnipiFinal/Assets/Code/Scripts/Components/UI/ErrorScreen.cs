using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorScreen : MonoBehaviour
{
    public static ErrorScreen Instance { get; private set; }

    [SerializeField] private GameObject _screen;
    [SerializeField] private TextMeshProUGUI _errorText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _errorText.text = "";
        _screen.SetActive(false);
    }

    public void Show(string msg)
    {
        _screen.SetActive(true);
        _errorText.text = "An error occured: " + msg;
    }
}
