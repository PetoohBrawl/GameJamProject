using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceInfo
{
    public DialogSequenceData DialogSequenceData { get; private set; }

    private bool _isCompleted;

    public DialogSequenceInfo(DialogSequenceData data)
    {
        DialogSequenceData = data;
    }

    public bool CanStartSequence()
    {
        if (_isCompleted)
        {
            return false;
        }

        // TODO: проверку condition

        return true;
    }

    public void SetCompleted()
    {
        _isCompleted = true;
    }
}
