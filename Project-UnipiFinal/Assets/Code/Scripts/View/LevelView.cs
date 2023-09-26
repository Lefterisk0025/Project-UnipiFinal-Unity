using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelView : MonoBehaviour
{
    LevelPresenter _levelPresenter;

    [Header("General Settings")]
    [SerializeField] internal GridView GridView;
    [SerializeField] internal LevelPerformanceView LevelPerformanceView;

    [Header("Diffuculty configs")]
    [SerializeField] private List<TimeAttackConfig> _timeAttackDifficultyConfigs;
    [SerializeField] private List<MatchPointConfig> _matchPointDifficultyConfigs;

    [Header("UI")]
    [SerializeField] private TextCountdownTimer _preGameTimer;
    [SerializeField] private TextCountdownTimer _centralLevelTimer;
    [SerializeField] private BarCountdownTimer _repeatBarTimer;

    [HideInInspector] public UnityEvent OnViewInitialized;
    [HideInInspector] public UnityEvent OnViewDisabled;
    [HideInInspector] public UnityEvent PreGameTimerEnded;
    [HideInInspector] public UnityEvent CentralLevelTimerEnded;
    [HideInInspector] public UnityEvent RepeatBarTimerEnded;

    /// <summary>
    /// Use bool to show if its victory or not
    /// </summary>
    [HideInInspector] public UnityEvent<bool> OnLevelEnd;
    /// <summary>
    /// Use bool to show if its victory or not
    /// </summary>
    public UnityEvent<bool> OnMissionEnd;

    private void Awake()
    {
        _levelPresenter = new LevelPresenter(this);
    }

    private void OnEnable()
    {
        GameManager.Instance.EnableMainCamera();

        LoadingScreen.Instance.OnLoadFinish.AddListener(DisplayAndStartPreGameTimer);

        _preGameTimer.gameObject.SetActive(false);
        _centralLevelTimer.gameObject.SetActive(false);
        _repeatBarTimer.gameObject.SetActive(false);

        OnViewInitialized.Invoke();
    }

    private void OnDisable()
    {
        OnViewDisabled.Invoke();
    }

    public void DisplayAndStartPreGameTimer()
    {
        _preGameTimer.gameObject.SetActive(true);

        LoadingScreen.Instance.OnLoadFinish.RemoveListener(DisplayAndStartPreGameTimer);

        _preGameTimer.OnTimerEnd.AddListener(() => PreGameTimerEnded.Invoke());

        StartCoroutine(_preGameTimer.StartCountDown(3));
    }

    public void DisplayRepeatBarTimer()
    {
        _repeatBarTimer.gameObject.SetActive(true);

        _repeatBarTimer.OnTimerEnd.AddListener(() => RepeatBarTimerEnded.Invoke());
        LevelPerformanceView.OnAllMatchesFound.AddListener(() => _repeatBarTimer.StartAndRepeatBarTimer());
    }

    public void StartRepeatBarTimer(int durationTilRepeat)
    {
        _repeatBarTimer.InitializeRepeatTimer(durationTilRepeat);
        _repeatBarTimer.StartAndRepeatBarTimer();
    }

    public void DisplayCentralLevelTimer(int duration)
    {
        _centralLevelTimer.gameObject.SetActive(true);
        _centralLevelTimer.OnTimerEnd.AddListener(() => CentralLevelTimerEnded.Invoke());
        _centralLevelTimer.InitializeDisplayOfCountDownInTimeFormatMinutes(duration);
    }

    public void StartCentralLevelTimer(int duration)
    {
        StartCoroutine(_centralLevelTimer.StartCountDownInTimeFormatMinutes(duration));
    }

    public void DisableTimers()
    {
        _repeatBarTimer.StopTimer();
        StopAllCoroutines(); // for stopping coroutine timers

        _repeatBarTimer.OnTimerEnd.RemoveAllListeners();
        _centralLevelTimer.OnTimerEnd.RemoveAllListeners();
        _preGameTimer.OnTimerEnd.RemoveAllListeners();
    }

    public void AbandonLevel()
    {
        _levelPresenter.HandleAbandonLevel();
    }

    public void ContinueLevel()
    {
        _levelPresenter.HandleContinueLevel();
    }

    public MatchConfig GetMatchConfigByDifficulty(GameMode gameMode, Difficulty difficulty)
    {
        switch (gameMode)
        {
            case GameMode.TimeAttack:
                foreach (var config in _timeAttackDifficultyConfigs)
                {
                    if (config.Difficulty == difficulty)
                    {
                        return config;
                    }
                }
                return null;
            case GameMode.MatchPoint:
                foreach (var config in _matchPointDifficultyConfigs)
                {
                    if (config.Difficulty == difficulty)
                    {
                        return config;
                    }
                }
                return null;
        }

        return null;
    }
}
