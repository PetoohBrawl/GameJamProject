using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class CharacterData
{
    public string Name { get; private set; }
    public int StartReputation { get; private set; }
    public List<DialogSequenceData> DialogSequenceDatas { get; private set; } = new List<DialogSequenceData>();

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        StartReputation = data.GetInt("StartReputation");

        string[] dialogSequencesNames = ((string)data["DialogSequences"]).Split('\n');

        foreach (string dialogSequenceName in dialogSequencesNames)
        {
            DialogSequenceData sequenceData = GameDataStorage.Instance.GetDialogSequenceData(dialogSequenceName);

            DialogSequenceDatas.Add(sequenceData);
        }
    }
}
