using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TextCountdownTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _countdownText;
    [HideInInspector] public UnityEvent OnTimerEnd;

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
    }

    public IEnumerator StartCountDownInTimeFormat(int timeInSec)
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

        OnTimerEnd.Invoke();

        _countdownText.gameObject.SetActive(false);
    }
}
