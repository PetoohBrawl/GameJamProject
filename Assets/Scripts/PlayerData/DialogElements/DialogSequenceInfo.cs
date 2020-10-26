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

        // TODO: проверку condition

        return true;
    }

    public void SetCompleted()
    {
        IsCompleted = true;
    }
}
