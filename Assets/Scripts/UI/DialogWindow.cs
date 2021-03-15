using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _phraseText;
    [SerializeField] private RectTransform _buttonsParent;
    [SerializeField] private ChoiceButton _buttonPrefab;

    private DialogSequenceInfo _currentSequenceInfo;
    private DialogStageInfo _currentStage;
    private readonly List<DialogStageInfo> _currentStages = new List<DialogStageInfo>();

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
        _currentStages.Clear();

        DialogStageInfo startStage;

        if (dialogSequenceInfo.IsCompleted == false)
        {
            startStage = _currentSequenceInfo.StartStageInfo;
        }
        else
        {
            startStage = _currentSequenceInfo.FinalStageInfo;
        }

        SetupView(startStage);
    }

    private void SetupView(DialogChoiceData choiceData)
    {
        DialogStageData nextStageData;

        if (choiceData == null)
        {
            nextStageData = DialogStagesDataStorage.Instance.GetByName(_currentStage.Data.NextStageName);
        }
        else
        {
            nextStageData = DialogStagesDataStorage.Instance.GetByName(choiceData.StageName);
            _currentStage.TryRemoveChoice(choiceData);
        }

        DialogStageInfo nextStageInfo = null;

        if (nextStageData != null)
        {
            nextStageInfo = _currentStages.Find((stageInfo) => stageInfo.Data.Equals(nextStageData));

            if (nextStageInfo == null)
            {
                nextStageInfo = new DialogStageInfo(nextStageData);
                _currentStages.Add(nextStageInfo);
            }
        }

        if (nextStageInfo == null)
        {
            SetupView(nextStageInfo);
        }
        else
        {
            gameObject.SetActive(false);

            LocationManager.Instance.SetupLocation(nextStageData.Location, null, () =>
            {
                SetupView(nextStageInfo);
                gameObject.SetActive(true);
            });
        }
    }

    private void SetupView(DialogStageInfo dialogStageInfo)
    {
        if (dialogStageInfo == null)
        {
            _currentSequenceInfo.SetCompleted();
            gameObject.SetActive(false);
            return;
        }

        Debug.LogWarning($"Stage {dialogStageInfo.Data.Name} initialized");

        _currentStage = dialogStageInfo;
        DialogStageData data = _currentStage.Data;

        PlayerProgress.Instance.RecordDiary(data.DiaryRecord);

        _phraseText.text = data.Phrase;
        _buttonsParent.DestroyChildren();

        List<DialogChoiceData> choices = _currentStage.GetActiveChoices();

        if (choices != null)
        {
            foreach (DialogChoiceData choiceData in choices)
            {
                ChoiceButton button = Instantiate(_buttonPrefab, _buttonsParent);
                button.SetupButton(choiceData);
            }
        }
        else
        {
            ChoiceButton button = Instantiate(_buttonPrefab, _buttonsParent);

            bool isEndOfDialog = string.IsNullOrEmpty(data.NextStageName);
            button.SetupButton(isEndOfDialog);
        }
    }
}
