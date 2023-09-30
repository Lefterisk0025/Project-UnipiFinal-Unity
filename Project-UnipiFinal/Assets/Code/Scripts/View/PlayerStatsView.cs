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
            _avatarFrame.transform.Find("GenderIcon").GetComponent<Image>().sprite = _maleGender;
        else if (player.Gender == 1)
            _avatarFrame.transform.Find("GenderIcon").GetComponent<Image>().sprite = _femaleGender;
        else
            _avatarFrame.transform.Find("GenderIcon").GetComponent<Image>().sprite = _noGender;
    }

    public void ShowAvatarFrame() => _avatarFrame.SetActive(true);
    public void HideAvatarFrame() => _avatarFrame.SetActive(false);
    public void ShowPerformanceStats() => _statsFrame.SetActive(true);
    public void HidePerformanceStats() => _statsFrame.SetActive(false);
}
