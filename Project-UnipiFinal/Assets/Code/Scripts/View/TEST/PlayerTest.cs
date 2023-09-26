using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerTest : MonoBehaviour
{
    PlayerRemoteService _playerRemoteService;

    [SerializeField] private TMP_Text _displayNameText;
    [SerializeField] private TMP_Text _userIdText;
    [SerializeField] private TMP_Text _reputationText;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_InputField _repAmountInputField;
    [SerializeField] private Button _addRepButton;

    private void OnEnable()
    {
        _errorText.gameObject.SetActive(false);
        _playerRemoteService = new PlayerRemoteService();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (PlayerManager.Instance.Player != null)
        {
            _displayNameText.text = "Display Name: " + PlayerManager.Instance.Player.DisplayName;
            _userIdText.text = "User ID: " + PlayerManager.Instance.Player.UserId;
            _reputationText.text = "Reputation: " + PlayerManager.Instance.Player.Reputation.ToString();
        }
        else
        {
            _displayNameText.text = "There is no player...";
        }
    }

    public async void OnAddReputationButtonClicked()
    {
        int rep = int.Parse(_repAmountInputField.text);

        PlayerManager.Instance.Player.Reputation += rep;
        PlayerManager.Instance.Player.NetCoins += 25;
        UpdateUI();

        if (await _playerRemoteService.UpdatePlayer(PlayerManager.Instance.Player))
            Debug.Log("Success!");

        else
        {
            _errorText.gameObject.SetActive(true);
            _errorText.text = "An error occured while updating reputation";
        }
    }
}
