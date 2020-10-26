using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class DialogSequenceData
{
    public string Name { get; private set; }
    public List<DialogStageData> DialogStages { get; private set; } = new List<DialogStageData>();
    public DialogStageData FinalStage { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];

        string[] dialogStagesNames = ((string)data["DialogStages"]).Split('\n');

        foreach (string dialogStageName in dialogStagesNames)
        {
            DialogStages.Add(GameDataStorage.Instance.GetDialogStageData(dialogStageName));
        }

        FinalStage = GameDataStorage.Instance.GetDialogStageData((string)data["FinalStage"]);
    }
}
