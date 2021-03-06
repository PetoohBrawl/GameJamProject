﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceInfo
{
    public readonly DialogStageInfo StartStageInfo;
    public readonly DialogStageInfo FinalStageInfo;
    public bool IsCompleted { get; private set; }
    public DialogSequenceData DialogSequenceData { get; private set; }
    
    public DialogSequenceInfo(DialogSequenceData data)
    {
        DialogSequenceData = data;

        StartStageInfo = new DialogStageInfo(DialogSequenceData.StartStage);

        if (DialogSequenceData.FinalStage != null)
        {
            FinalStageInfo = new DialogStageInfo(DialogSequenceData.FinalStage);
        }
    }

    public bool CanStartSequence()
    {
        if (IsCompleted)
        {
            return false;
        }

        // TODO: подумать о структуре кондишнов, обговорить с ГД
        if (DialogSequenceData.NeedToCompleteSequences != null)
        {
            foreach (string needToCompleteSequenceName in DialogSequenceData.NeedToCompleteSequences)
            {
                if (PlayerProgress.Instance.IsDialogSequenceCompleted(needToCompleteSequenceName) == false)
                {
                    return false;
                }
            }
        }

        CharacterInfo characterInfo = PlayerProgress.Instance.GetCharacterInfo(DialogSequenceData.ReputationTarget);

        switch (DialogSequenceData.ConditionDirection)
        {
            case 1:
                return characterInfo.ReputationValue >= DialogSequenceData.ReputationValue;

            case -1:
                return characterInfo.ReputationValue <= DialogSequenceData.ReputationValue;
        }

        return true;
    }

    public void SetCompleted()
    {
        PlayerProgress.Instance.CompleteDialogSequence(this);

        IsCompleted = true;
    }
}
