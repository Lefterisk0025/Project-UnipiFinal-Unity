using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DentedPixel;
using UnityEngine.Events;
using System;

public class MatchTimer : MonoBehaviour
{
    public UnityEvent OnTimerEnd;

    [SerializeField] private GameObject _timerBar;

    bool isTimerActive = false;
    int _durationTilRepeat;
    RectTransform _timerBarRectTransform;

    public void InitializeTimeAttackTimer(int durationTilRepeat)
    {
        _durationTilRepeat = durationTilRepeat;

        InitializeTimer();
    }

    private void InitializeTimer()
    {
        isTimerActive = true;
        _timerBarRectTransform = _timerBar.gameObject.GetComponent<RectTransform>();
        _timerBarRectTransform.localScale = new Vector3(0, _timerBar.transform.localScale.y);
    }

    public void StartTimer(int duration)
    {
        if (!isTimerActive)
            return;

        _timerBarRectTransform.localScale = new Vector3(0, _timerBar.transform.localScale.y);

        LeanTween.scaleX(_timerBar, 1, duration);
    }

    public void StartAndRepeatTimer()
    {
        if (!isTimerActive)
            return;

        LeanTween.cancel(_timerBar);
        _timerBarRectTransform.localScale = new Vector3(0, _timerBar.transform.localScale.y);

        LeanTween.scaleX(_timerBar, 1, _durationTilRepeat).setOnComplete(() =>
        {
            OnTimerEnd.Invoke();
            StartAndRepeatTimer();
        });
    }

    public void StopTimer()
    {
        isTimerActive = false;
    }
}
