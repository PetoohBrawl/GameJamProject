using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo
{
    public int reputationCounter;
    public List<DialogSequenceInfo> dialogSequences = new List<DialogSequenceInfo>();

    private CharacterData characterData;

    public CharacterInfo(CharacterData data)
    {
        characterData = data;
        reputationCounter = 0;

        foreach (DialogSequenceData dialogSequenceData in characterData.DialogSequenceDatas)
        {
            dialogSequences.Add(new DialogSequenceInfo(dialogSequenceData));
        }
    }
}
