using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    bool _timerOn;
    float _timeValue;

    public void SetTimer(float timeValue)
    {
        _timeValue = timeValue;
        _timerOn = true;
    }

    private void Update()
    {
        if (!_timerOn)
            return;

        if (_timeValue > 0)
            _timeValue -= Time.deltaTime;
        else
            _timeValue = 0;

        DisplayTime(_timeValue);
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
            timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
