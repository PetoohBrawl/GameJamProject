using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsDialog : MonoBehaviour
{
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Slider _volumeSlider;

    public static event Action<float> OnVolumeChange;

    public void ShowDialog()
    {
        _musicToggle.isOn = MusicController.Instance.IsMusicEnabled;
        _volumeSlider.value = MusicController.Instance.MusicVolume;

        gameObject.SetActive(true);
    }

    public void OnSliderValueChange()
    {
        OnVolumeChange?.Invoke(_volumeSlider.value);
    }

    public void OnMusicToggle()
    {
        MusicController.Instance.IsMusicEnabled = _musicToggle.isOn;
    }
}
