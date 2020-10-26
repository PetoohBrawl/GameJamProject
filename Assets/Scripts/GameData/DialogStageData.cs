using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class DialogStageData
{
    public string Name { get; private set; }
    public string Phrase { get; private set; }
    // переделать без инициализации по умолчанию
    public List<DialogChoiceData> DialogChoices { get; private set; } = new List<DialogChoiceData>();
    public string NextStageName { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        Phrase = (string)data["Phrase"];
        NextStageName = (string)data["NextStageName"];

        string choices = (string)data["Choices"];

        if (!string.IsNullOrEmpty(choices))
        {
            string[] choicesNames = choices.Split(',');

            foreach (string choiceName in choicesNames)
            {
                DialogChoices.Add(GameDataStorage.Instance.GetDialogChoiceData(choiceName));
            }
        }
    }
}
