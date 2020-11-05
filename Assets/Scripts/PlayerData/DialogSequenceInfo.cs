using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceInfo
{
    public readonly DialogStageInfo StartStageInfo;
    public readonly DialogStageInfo FinalStageInfo;
    public bool IsCompleted { get; private set; }
    public DialogSequenceData DialogSequenceData { get; private set; }

    public readonly string DialogOwner;
    
    public DialogSequenceInfo(DialogSequenceData data, string dialogOwner)
    {
        DialogSequenceData = data;
        DialogOwner = dialogOwner;

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
                if (PlayerInfo.Instance.IsDialogSequenceCompleted(needToCompleteSequenceName) == false)
                {
                    return false;
                }
            }
        }

        CharacterInfo characterInfo = GameController.Instance.GetCharacterInfo(DialogSequenceData.ReputationTarget);

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
        PlayerInfo.Instance.CompleteDialogSequence(this);

        IsCompleted = true;
    }
}
