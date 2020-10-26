using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo
{
    public int ReputationCounter;
    public List<DialogSequenceInfo> DialogSequences = new List<DialogSequenceInfo>();

    private CharacterData _characterData;

    public CharacterInfo(CharacterData data)
    {
        _characterData = data;
        ReputationCounter = 0;

        foreach (DialogSequenceData dialogSequenceData in _characterData.DialogSequenceDatas)
        {
            DialogSequences.Add(new DialogSequenceInfo(dialogSequenceData));
        }
    }
}
