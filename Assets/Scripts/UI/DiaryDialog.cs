using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryDialog : MonoBehaviour
{
    [SerializeField] private ScrollRect _recordsScroll;
    [SerializeField] private TextMeshProUGUI _recordsText;
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
}
