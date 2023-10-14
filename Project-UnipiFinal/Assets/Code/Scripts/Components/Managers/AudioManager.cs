using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Musics")]
    [SerializeField] private AudioSource _mainMenu;
    [SerializeField] private AudioSource _missionFailed;
    [SerializeField] private AudioSource _missionCompleted;

    [Header("Effects")]
    [SerializeField] private AudioSource _matchFound;
    [SerializeField] private AudioSource _matchFailed;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void PlayMainMenuMusic()
    {
        if (!_mainMenu.isPlaying)
            _mainMenu.Play();
    }

    public void StopMainMenuMusic()
    {
        _mainMenu.Stop();
    }

    public void PlayMissionFailedMusic()
    {
        if (!_missionFailed.isPlaying)
            _missionFailed.Play();
    }

    public void PlayMissionCompletedMusic()
    {
        if (!_missionCompleted.isPlaying)
            _missionCompleted.Play();
    }

    public void PlayMatchFoundEffect()
    {
        if (_matchFound.isPlaying)
            _matchFound.Stop();

        _matchFound.Play();
    }

    public void PlayMatchFailedEffect()
    {
        if (_matchFailed.isPlaying)
            _matchFailed.Stop();

        _matchFailed.Play();
    }

    public void UnMuteAllEffects()
    {
        _matchFound.mute = true;
        _matchFailed.mute = true;
    }

    public void MuteAllEffects()
    {
        _matchFound.mute = false;
        _matchFailed.mute = false;
    }

    public void UnMuteAllMusics()
    {
        _mainMenu.mute = true;
        _missionCompleted.mute = true;
        _missionFailed.mute = true;

    }

    public void MuteAllMusics()
    {
        _mainMenu.mute = false;
        _missionCompleted.mute = false;
        _missionFailed.mute = false;
    }

    public void StopAllMusics()
    {
        _mainMenu.Stop();
        _missionCompleted.Stop();
        _missionFailed.Stop();
    }

    public void StopAllEffects()
    {
        _matchFound.Stop();
        _matchFailed.Stop();
    }
}
