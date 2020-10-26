using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo
{
    public int ReputationCounter;
    public List<DialogSequenceInfo> DialogSequences = new List<DialogSequenceInfo>();

    private CharacterData charac_terData;

    public CharacterInfo(CharacterData data)
    {
        charac_terData = data;
        ReputationCounter = 0;

        foreach (DialogSequenceData dialogSequenceData in charac_terData.DialogSequenceDatas)
        {
            DialogSequences.Add(new DialogSequenceInfo(dialogSequenceData));
        }
    }
}
