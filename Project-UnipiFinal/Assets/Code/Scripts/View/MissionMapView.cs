using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionMapView : MonoBehaviour
{
    [SerializeField] private MissionDataSO _missionDataSO;

    [SerializeField] private TextMeshProUGUI missionTitleGUI;
    [SerializeField] private TextMeshProUGUI missionDifficultyGUI;

    private void Start()
    {
        missionTitleGUI.text = "Mission: " + _missionDataSO.Title;
        missionDifficultyGUI.text = "Difficulty: " + _missionDataSO.Difficulty.ToString();
    }

    public void SetMissionView()
    {

    }
}
