using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class DialogStageData
{
    public string Name { get; private set; }
    public string Phrase { get; private set; }
    public string DiaryRecord { get; private set; }
    // переделать без инициализации по умолчанию
    public List<DialogChoiceData> DialogChoices { get; private set; } = new List<DialogChoiceData>();
    public string NextStageName { get; private set; }
    public LocationName Location { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        Phrase = (string)data["Phrase"];
        NextStageName = (string)data["NextStageName"];

        if (string.IsNullOrEmpty(NextStageName))
        {
            if (GameDataStorage.Instance.GetDialogStageData(NextStageName) == null)
            {
                Debug.LogError($"NEXT_STAGE_NAME is NULL with NAME: {NextStageName}, DIALOG_STAGE: {Name}");
            }
        }

        DiaryRecord = (string)data["DiaryRecord"];

        string choices = (string)data["Choices"];

        if (!string.IsNullOrEmpty(choices))
        {
            string[] choicesNames = choices.Split('\n');

            foreach (string choiceName in choicesNames)
            {
                DialogChoiceData choiceData = GameDataStorage.Instance.GetDialogChoiceData(choiceName);

                if (choiceData == null)
                {
                    Debug.LogError($"CHOICE_DATA is NULL with NAME: {choiceName}, DIALOG_STAGE: {Name}");
                }

                DialogChoices.Add(choiceData);
            }
        }

        Location = data.GetEnum((string)data["Location"], LocationName.Unknown);
    }
}
