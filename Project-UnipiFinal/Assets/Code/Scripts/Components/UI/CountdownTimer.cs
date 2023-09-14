using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{

    [SerializeField] private TMP_Text _countdownText;

    public UnityEvent OnCountdownEnd;

    private void Start()
    {
        _countdownText.text = "";
        _countdownText.gameObject.SetActive(false);
    }

    public IEnumerator StartCountDown(int value)
    {
        _countdownText.gameObject.SetActive(true);
        _countdownText.text = value.ToString();

        do
        {
            yield return new WaitForSeconds(1);
            value--;
            _countdownText.text = value.ToString();
        }
        while (value > 0);

        _countdownText.gameObject.SetActive(false);

        OnCountdownEnd.Invoke();
    }

    public IEnumerator StartCountDownTime(int timeInSec)
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

        OnCountdownEnd.Invoke();
    }
}
