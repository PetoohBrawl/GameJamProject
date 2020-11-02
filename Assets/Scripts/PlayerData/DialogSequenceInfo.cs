using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceInfo
{
    public readonly DialogStageInfo StartStageInfo;
    public readonly DialogStageInfo FinalStageInfo;
    public bool IsCompleted { get; private set; }

    public readonly string DialogOwner;

    private DialogSequenceData _dialogSequenceData;

    public DialogSequenceInfo(DialogSequenceData data, string dialogOwner)
    {
        _dialogSequenceData = data;
        DialogOwner = dialogOwner;

        StartStageInfo = new DialogStageInfo(_dialogSequenceData.StartStage);
        FinalStageInfo = new DialogStageInfo(_dialogSequenceData.FinalStage);
    }

    public bool CanStartSequence()
    {
        if (IsCompleted)
        {
            return false;
        }

        // TODO: переделать на универсальный метод проверки кондишна
        CharacterInfo characterInfo = GameController.Instance.GetCharacterInfo(_dialogSequenceData.ReputationTarget);

        switch (_dialogSequenceData.ConditionDirection)
        {
            case 1:
                return characterInfo.ReputationValue >= _dialogSequenceData.ReputationValue;

            case -1:
                return characterInfo.ReputationValue <= _dialogSequenceData.ReputationValue;
        }

        return true;
    }

    public void SetCompleted()
    {
        IsCompleted = true;
    }
}
