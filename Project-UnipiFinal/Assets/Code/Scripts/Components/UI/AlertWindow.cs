using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindow : MonoBehaviour
{
    public static AlertWindow Instance { get; private set; }

    [Header("Message Alert")]
    [SerializeField] private GameObject _msgAlertPanel;
    [SerializeField] private TMP_Text _msgTitle;
    [SerializeField] private TMP_Text _msgDescription;

    [Header("Yes/No Alert")]
    [SerializeField] private GameObject _yesnoAlertPanel;
    [SerializeField] private TMP_Text _yesnoTitle;
    [SerializeField] private TMP_Text _yesnoDescription;
    [SerializeField] private Button _yesBtn;
    [SerializeField] private Button _noBtn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _msgAlertPanel.SetActive(false);
        _yesnoAlertPanel.SetActive(false);
    }

    public void ShowMessageAlert(string title, string body)
    {
        _msgAlertPanel.SetActive(true);
        _msgTitle.text = title;
        _msgDescription.text = body;
    }

    public void OnCloseButtonClicked()
    {
        _msgAlertPanel.SetActive(false);
    }

    public void ShowYesNoAlert(string title, string body, Action yesAction, Action noAction)
    {
        _yesnoAlertPanel.SetActive(true);
        _yesnoTitle.text = title;
        _yesnoDescription.text = body;

        _yesBtn.onClick.AddListener(() =>
        {
            HideYesNoAlert();
            yesAction();
        });
        _noBtn.onClick.AddListener(() =>
        {
            HideYesNoAlert();
            noAction();
        });
    }

    private void HideYesNoAlert()
    {
        _yesnoAlertPanel.SetActive(false);
    }
}
