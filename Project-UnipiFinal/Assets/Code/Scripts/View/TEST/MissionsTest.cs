using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionsTest : MonoBehaviour
{
    MissionRemoteService _missionRemoteService;

    [SerializeField] private MissionCard _missionCardPrefab;
    [SerializeField] private Transform _contentParent;
    [SerializeField] private TMP_InputField _missionsAmountInputField;
    [SerializeField] private TMP_Text _errorText;

    private void OnEnable()
    {
        _errorText.gameObject.SetActive(false);
        _missionRemoteService = new MissionRemoteService();
    }

    public async void OnFetchMissionButtonClicked()
    {
        _errorText.gameObject.SetActive(false);

        int amount = int.Parse(_missionsAmountInputField.text);
        if (amount <= 0 || amount >= 10)
            return;

        int i = 0;
        List<Mission> missionsList = null;
        try
        {
            missionsList = new List<Mission>(await _missionRemoteService.GetRandomRemoteMissions(amount));

            i = 1;
            if (missionsList.Count == 0)
            {
                _errorText.gameObject.SetActive(true);
                _errorText.text = "There are no missions";
                return;
            }
            i = 2;

            if (_contentParent.childCount > 0)
            {
                foreach (Transform item in _contentParent)
                    Destroy(item.gameObject);
            }

            MissionCard missionCardView;
            foreach (var mission in missionsList)
            {
                var spawnedMission = Instantiate(_missionCardPrefab, _contentParent);
                missionCardView = spawnedMission.GetComponent<MissionCard>();
                missionCardView.SetMissionCardView(mission);
            }
            i = 3;
        }
        catch (Exception e)
        {
            _errorText.gameObject.SetActive(true);
            _errorText.text = e.Message + $" ({i})";
        }
    }
}
