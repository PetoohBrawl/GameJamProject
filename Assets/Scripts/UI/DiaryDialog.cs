using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryDialog : MonoBehaviour
{
    [SerializeField] private ScrollRect _recordsScroll;
    [SerializeField] private TextMeshProUGUI _recordsText;

    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _sliderValue;

    public static event Action<float> OnVolumeChange;
    public void ShowDialog()
    {
        gameObject.SetActive(true);

        _recordsText.text = PlayerInfo.Instance.DialogHistory.ToString();
        _recordsText.ForceMeshUpdate();

        _recordsScroll.normalizedPosition = Vector2.one;
    }

    public void CloseDialog()
    {
        gameObject.SetActive(false);
    }

    public void OnSliderValueChange()
    {
        OnVolumeChange?.Invoke(_volumeSlider.value);
        _sliderValue.text = _volumeSlider.value.ToString();
    }
}
