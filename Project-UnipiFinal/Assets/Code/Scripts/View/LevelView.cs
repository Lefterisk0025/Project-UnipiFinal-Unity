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
    [HideInInspector] public UnityEvent OnPerformanceStatsPrepared;
    [HideInInspector] public UnityEvent PreGameTimerEnded;
    [HideInInspector] public UnityEvent CentralLevelTimerEnded;
    [HideInInspector] public UnityEvent RepeatBarTimerEnded;
    [HideInInspector] public UnityEvent<bool> OnLevelEndedVictorious;
    public UnityEvent OnLevelLost;
    public UnityEvent OnLevelWin;

    private void Awake()
    {
        _levelPresenter = new LevelPresenter(this);
    }

    private void OnEnable()
    {
        // Setup events
        LoadingScreen.Instance.OnLoadFinish.AddListener(DisplayAndStartPreGameTimer);

        // Disable UI
        _preGameTimer.gameObject.SetActive(false);
        _centralLevelTimer.gameObject.SetActive(true);

        GameManager.Instance.EnableMainCamera();

        OnViewInitialized.Invoke();
    }

    private void OnDisable()
    {
        LoadingScreen.Instance.OnLoadFinish.RemoveListener(DisplayAndStartPreGameTimer);
        _preGameTimer.OnTimerEnd.RemoveListener(() => PreGameTimerEnded.Invoke());
        LevelPerformanceView.OnAllMatchesFound.RemoveListener(() => _repeatBarTimer.StartAndRepeatBarTimer());
        _repeatBarTimer.OnTimerEnd.RemoveListener(() => RepeatBarTimerEnded.Invoke());
        _centralLevelTimer.OnTimerEnd.RemoveListener(() => CentralLevelTimerEnded.Invoke());

        OnViewDisabled.Invoke();
    }

    public void DisplayAndStartPreGameTimer()
    {
        _preGameTimer.gameObject.SetActive(true);

        _preGameTimer.OnTimerEnd.AddListener(() => PreGameTimerEnded.Invoke());
        LevelPerformanceView.OnAllMatchesFound.AddListener(() => _repeatBarTimer.StartAndRepeatBarTimer());

        StartCoroutine(_preGameTimer.StartCountDown(3));
    }

    public void DisplayAndStartRepeatBarTimer(int durationTilRepeat)
    {
        _repeatBarTimer.gameObject.SetActive(true);

        _repeatBarTimer.OnTimerEnd.AddListener(() => RepeatBarTimerEnded.Invoke());

        _repeatBarTimer.InitializeRepeatTimer(durationTilRepeat);
        _repeatBarTimer.StartAndRepeatBarTimer();
    }

    public void DisplayAndStartCentralLevelTimer(int duration)
    {
        _centralLevelTimer.gameObject.SetActive(true);
        _centralLevelTimer.OnTimerEnd.AddListener(() => CentralLevelTimerEnded.Invoke());
        StartCoroutine(_centralLevelTimer.StartCountDownInTimeFormatMinutes(duration));
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
