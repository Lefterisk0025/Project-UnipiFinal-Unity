using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System;

public class TextCountdownTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _countdownText;
    [HideInInspector] public UnityEvent OnTimerEnd;

    private void OnEnable()
    {
        _countdownText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {

    }

    public IEnumerator StartCountDown(int timeValue)
    {
        _countdownText.gameObject.SetActive(true);

        int timeRemaining = timeValue;
        _countdownText.text = timeRemaining.ToString();

        do
        {
            yield return new WaitForSeconds(1);
            timeRemaining--;
            _countdownText.text = timeRemaining.ToString();
        }
        while (timeRemaining > 0);

        OnTimerEnd.Invoke();

        _countdownText.gameObject.SetActive(false);
        OnTimerEnd.RemoveAllListeners();
    }

    public void InitializeDisplayOfCountDownInTimeFormatMinutes(int timeInSec)
    {
        _countdownText.gameObject.SetActive(true);

        int timeRemaining = timeInSec;
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        _countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public IEnumerator StartCountDownInTimeFormatMinutes(int timeInSec)
    {
        _countdownText.gameObject.SetActive(true);

        int timeRemaining = timeInSec;
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);

        _countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        while (timeRemaining > 0)
        {
            minutes = Mathf.FloorToInt(timeRemaining / 60);
            seconds = Mathf.FloorToInt(timeRemaining % 60);
            timeRemaining--;
            _countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(1);
        }

        _countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        OnTimerEnd.Invoke();

        _countdownText.gameObject.SetActive(false);
    }

    public void InitializeDisplayOfCountDownInTimeFormatHours(int timeInSec)
    {
        _countdownText.gameObject.SetActive(true);

        int timeRemaining = timeInSec;
        int hours = Mathf.FloorToInt(timeRemaining / 3600);
        int minutes = Mathf.FloorToInt(timeRemaining / 60) % 60;
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        _countdownText.text = $"{hours}:{minutes:D2}:{seconds:D2}";
    }

    public IEnumerator StartCountDownInTimeFormatHours(int timeInSec)
    {
        _countdownText.gameObject.SetActive(true);

        int timeRemaining = timeInSec;
        int hours = Mathf.FloorToInt(timeRemaining / 3600);
        int minutes = Mathf.FloorToInt(timeRemaining / 60) % 60;
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        _countdownText.text = $"{hours}:{minutes:D2}:{seconds:D2}";

        while (timeRemaining > 0)
        {
            hours = Mathf.FloorToInt(timeRemaining / 3600);
            minutes = Mathf.FloorToInt(timeRemaining / 60) % 60;
            seconds = Mathf.FloorToInt(timeRemaining % 60);
            timeRemaining--;

            _countdownText.text = $"{hours}:{minutes:D2}:{seconds:D2}";

            yield return new WaitForSeconds(1);
        }

        _countdownText.text = $"{hours}:{minutes:D2}:{seconds:D2}";

        OnTimerEnd.Invoke();

        _countdownText.gameObject.SetActive(false);
    }
}
