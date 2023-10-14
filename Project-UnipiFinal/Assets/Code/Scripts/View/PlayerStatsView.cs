using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsView : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayNameText;
    [SerializeField] private TMP_Text _reputationValueText;
    [SerializeField] private TMP_Text _netCoinsValueText;
    [SerializeField] private GameObject _avatarFrame;
    [SerializeField] private Image _genderIcon;
    [SerializeField] private GameObject _statsFrame;
    [SerializeField] private Sprite _maleGender;
    [SerializeField] private Sprite _femaleGender;
    [SerializeField] private Sprite _noGender;

    public void DisplayPlayerInformation(Player player)
    {
        _displayNameText.text = player.DisplayName;
        _reputationValueText.text = player.Reputation.ToString();
        _netCoinsValueText.text = player.NetCoins.ToString();

        if (player.Gender == 0)
            _genderIcon.sprite = _maleGender;
        else if (player.Gender == 1)
            _genderIcon.sprite = _femaleGender;
        else
            _genderIcon.sprite = _noGender;
    }

    public void ShowAvatar() => _avatarFrame.SetActive(true);
    public void HideAvatar() => _avatarFrame.SetActive(false);
    public void ShowPerformanceStats() => _statsFrame.SetActive(true);
    public void HidePerformanceStats() => _statsFrame.SetActive(false);
}
