using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Toggle _soundEffectsToggle;
    [SerializeField] private Toggle _musicToggle;

    private void Start()
    {
        _soundEffectsToggle.onValueChanged.AddListener(OnSoundEffectsToggleValueChaned);
        _musicToggle.onValueChanged.AddListener(OnMusicToggleValueChaned);
    }

    private void OnSoundEffectsToggleValueChaned(bool isOn)
    {
        if (isOn)
            AudioManager.Instance.MuteAllEffects();
        else
            AudioManager.Instance.UnMuteAllEffects();
    }

    public void OnMusicToggleValueChaned(bool isOn)
    {
        if (isOn)
            AudioManager.Instance.MuteAllMusics();
        else
            AudioManager.Instance.UnMuteAllMusics();
    }
}
