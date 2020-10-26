using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceInfo
{
    public DialogSequenceData DialogSequenceData { get; private set; }

    private bool isCompleted;

    public DialogSequenceInfo(DialogSequenceData data)
    {
        DialogSequenceData = data;
    }

    public bool CanStartSequence()
    {
        if (isCompleted)
        {
            return false;
        }

        // TODO: проверку condition

        return true;
    }

    public void SetCompleted()
    {
        isCompleted = true;
    }
}
