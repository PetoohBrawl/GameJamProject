﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _phraseText;
    [SerializeField]
    private RectTransform _buttonsParent;
    [SerializeField]
    private ChoiceButton _buttonPrefab;

    private DialogSequenceInfo _currentSequenceInfo;
    private List<DialogStageData> _currentStages;
    private DialogStageData _currentStage;

    private void OnEnable()
    {
        ChoiceButton.OnChoiceMade += SetupView;
    }

    private void OnDisable()
    {
        ChoiceButton.OnChoiceMade -= SetupView;
    }

    public void ShowDialog(DialogSequenceInfo dialogSequenceInfo)
    {
        gameObject.SetActive(true);

        _currentSequenceInfo = dialogSequenceInfo;

        DialogStageData startStage = _currentSequenceInfo.DialogSequenceData.DialogStages[0];

        SetupView(startStage);
    }

    private void SetupView(DialogStageData dialogStageData)
    {
        if (dialogStageData == null)
        {
            if (string.IsNullOrEmpty(_currentStage.NextStageName))
            {
                _currentSequenceInfo.SetCompleted();
                gameObject.SetActive(false);
                return;
            }

            dialogStageData = GameDataStorage.Instance.GetDialogStageData(_currentStage.NextStageName);
        }

        _currentStage = dialogStageData;

        _phraseText.text = _currentStage.Phrase;
        _buttonsParent.DestroyChildren();

        if (_currentStage.DialogChoices.Count > 0)
        {
            foreach (DialogChoiceData choiceData in _currentStage.DialogChoices)
            {
                ChoiceButton button = Instantiate(_buttonPrefab, _buttonsParent);
                button.SetupButton(choiceData);
            }
        }
        else
        {
            ChoiceButton button = Instantiate(_buttonPrefab, _buttonsParent);

            bool isEndOfDialog = string.IsNullOrEmpty(_currentStage.NextStageName);
            button.SetupButton(isEndOfDialog);
        }
    }
}