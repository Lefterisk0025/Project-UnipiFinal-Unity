using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BarCountdownTimer : MonoBehaviour
{
    [SerializeField] private GameObject _timerBar;
    [HideInInspector] public UnityEvent OnTimerEnd;
    RectTransform _timerBarRectTransform;
    int _durationTilRepeat;

    private void Awake()
    {
        _timerBarRectTransform = _timerBar.gameObject.GetComponent<RectTransform>();
    }

    public void InitializeRepeatTimer(int durationTilRepeat)
    {
        _durationTilRepeat = durationTilRepeat;
    }

    public void StartAndRepeatBarTimer()
    {
        LeanTween.cancel(_timerBar);

        _timerBarRectTransform.localScale = new Vector3(0, _timerBar.transform.localScale.y);

        LeanTween.scaleX(_timerBar, 1, _durationTilRepeat).setOnComplete(() =>
        {
            OnTimerEnd.Invoke();
            StartAndRepeatBarTimer();
        });
    }

    public void StopTimer()
    {
        LeanTween.cancel(_timerBar);
        _timerBarRectTransform.localScale = new Vector3(0, _timerBar.transform.localScale.y);
    }
}
