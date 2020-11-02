using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class DialogSequenceData
{
    public string Name { get; private set; }
    public DialogStageData StartStage { get; private set; }
    public DialogStageData FinalStage { get; private set; }
    public int HistoryStageNumber { get; private set; }

    // TODO: объединить в класс condition с общим методом на проверку
    public int ReputationValue { get; private set; }
    public string ReputationTarget { get; private set; }
    public int ConditionDirection { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];

        StartStage = GameDataStorage.Instance.GetDialogStageData((string)data["StartStage"]);
        FinalStage = GameDataStorage.Instance.GetDialogStageData((string)data["FinalStage"]);

        ConditionDirection = data.GetInt("ConditionDirection", 0);

        if (ConditionDirection != 0)
        {
            ReputationValue = data.GetInt("ReputationValue");
            ReputationTarget = (string)data["ReputationTarget"];
        }

        HistoryStageNumber = data.GetInt("HistoryStageNumber");
    }
}
