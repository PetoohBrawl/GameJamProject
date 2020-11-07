﻿using System.Collections;
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
    public string[] NeedToCompleteSequences { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];

        StartStage = GameDataStorage.Instance.GetDialogStageData((string)data["StartStage"]);

        if (StartStage == null)
        {
            Debug.LogError($"START_STAGE NULL with NAME: {(string)data["StartStage"]}, DIALOG_SEQUENCE: {Name}");
        }

        FinalStage = GameDataStorage.Instance.GetDialogStageData((string)data["FinalStage"]);

        if (FinalStage == null)
        {
            Debug.LogError($"FINAL_STAGE NULL with NAME: {(string)data["FinalStage"]}, DIALOG_SEQUENCE: {Name}");
        }

        ConditionDirection = data.GetInt("ConditionDirection", 0);

        if (ConditionDirection != 0)
        {
            ReputationValue = data.GetInt("ReputationValue");
            ReputationTarget = (string)data["ReputationTarget"];
        }

        HistoryStageNumber = data.GetInt("HistoryStageNumber");

        if (HistoryStageNumber <= 0)
        {
            Debug.LogError($"HISTORY_STAGE_NUMBER <= 0, DIALOG_SEQUENCE: {Name}");
        }

        string needToCompleteSequencesData = (string)data["NeedToCompleteSequences"];

        if (string.IsNullOrEmpty(needToCompleteSequencesData) == false)
        {
            NeedToCompleteSequences = needToCompleteSequencesData.Split('\n');
        }
    }
}
