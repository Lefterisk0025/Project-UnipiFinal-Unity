using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class MapStatsView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private TMP_Text _reputationValueText;
    [SerializeField] private TMP_Text _matchesValueText;
    [SerializeField] private TMP_Text _scoreValueText;

    bool _isOpen;
    BoxCollider2D _boxCollider;

    private void OnEnable()
    {
        if (_boxCollider == null)
            _boxCollider = this.GetComponent<BoxCollider2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _boxCollider.enabled = false;
        DisplayMissionStats();
        if (!_isOpen)
            ShowMissionStats();
        else
            HideMissionStats();
    }

    private void DisplayMissionStats()
    {
        _reputationValueText.text = "Reputation: " + PlayerPrefs.GetInt("MissionReputation").ToString();
        _matchesValueText.text = "Matches: " + PlayerPrefs.GetInt("MissionMatches").ToString();
        _scoreValueText.text = "Score: " + PlayerPrefs.GetInt("MissionScore").ToString();
    }

    private void ShowMissionStats()
    {
        LeanTween.moveLocalX(_statsPanel, -180, 1).setEaseOutQuart().setOnComplete(() =>
        {
            _isOpen = true;
            _boxCollider.enabled = true;
        });
    }

    private void HideMissionStats()
    {
        LeanTween.moveLocalX(_statsPanel, 135, 0.7f).setEaseOutQuart().setOnComplete(() =>
        {
            _isOpen = false;
            _boxCollider.enabled = true;
        });
    }
}
