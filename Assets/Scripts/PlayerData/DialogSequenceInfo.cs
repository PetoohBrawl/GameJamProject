﻿using System.Collections;
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
        IsCompleted = true;
    }
}
