using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelView : MonoBehaviour
{
    LevelPresenter _levelPresenter;

    [Header("Diffuculty configs")]
    [SerializeField] private List<TimeAttackConfig> _timeAttackDifficultyConfigs;
    [SerializeField] private List<MatchPointConfig> _matchPointDifficultyConfigs;

    [Header("General UI")]
    [SerializeField] private TextCountdownTimer _preGameTimer;

    [HideInInspector] public UnityEvent OnViewInitialized;
    [HideInInspector] public UnityEvent OnViewDisabled;
    [HideInInspector] public UnityEvent OnPreGameTimerEnd;

    private void Awake()
    {
        _levelPresenter = new LevelPresenter(this);
    }

    private void OnEnable()
    {
        _preGameTimer.OnTimerEnd.AddListener(HandlePreGameTimerEnded);

        OnViewInitialized.Invoke();
    }

    private void OnDisable()
    {
        _preGameTimer.OnTimerEnd.RemoveAllListeners();

        OnViewDisabled.Invoke();
    }

    public void DisplayPreGameTimer()
    {
        StartCoroutine(_preGameTimer.StartCountDown(3));
    }

    private void HandlePreGameTimerEnded()
    {
        OnPreGameTimerEnd.Invoke();
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
