using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatsView : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayNameText;
    [SerializeField] private TMP_Text _reputationValueText;
    [SerializeField] private TMP_Text _netCoinsValueText;
    [SerializeField] private GameObject _avatarFrame;
    [SerializeField] private GameObject _statsFrame;

    bool isAvatarOpen;

    public void DisplayPlayerInformation(Player player)
    {
        _displayNameText.text = player.DisplayName;
        _reputationValueText.text = player.Reputation.ToString();
        _netCoinsValueText.text = player.NetCoins.ToString();
    }

    public void ShowAvatarFrame() => _avatarFrame.SetActive(true);
    public void HideAvatarFrame() => _avatarFrame.SetActive(false);
    public void ShowPerformanceStats() => _statsFrame.SetActive(true);
    public void HidePerformanceStats() => _statsFrame.SetActive(false);
}
