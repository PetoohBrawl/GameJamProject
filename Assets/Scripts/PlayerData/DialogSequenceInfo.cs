using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceInfo
{
    public DialogSequenceData DialogSequenceData { get; private set; }
    public bool IsCompleted { get; private set; }
    
    public DialogSequenceInfo(DialogSequenceData data)
    {
        DialogSequenceData = data;
    }

    public bool CanStartSequence()
    {
        if (IsCompleted)
        {
            return false;
        }

        // TODO: переделать на универсальный метод проверки кондишна
        switch (DialogSequenceData.ConditionType)
        {
            case ImpactType.Reputation:
                CharacterInfo characterInfo = GameController.Instance.GetCharacterInfo(DialogSequenceData.ConditionTarget);

                if (characterInfo.ReputationValue < DialogSequenceData.ConditionValue)
                {
                    return false;
                }

                break;
        }

        return true;
    }

    public void SetCompleted()
    {
        IsCompleted = true;
    }
}
