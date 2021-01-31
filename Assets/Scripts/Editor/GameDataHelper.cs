using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using System.IO;
using UnityEditor;

public static class GameDataHelper
{
    public static JsonArray _charactersData;
    public static JsonArray _sequencesData;
    public static JsonArray _stagesData;
    public static JsonArray _choicesData;

    private static string _charactersDataPath;
    private static string _sequencesDataPath;
    private static string _stagesDataPath;
    private static string _choicesDataPath;

    private static bool _isDirty = false;

    public static void Init()
    {
        string assetDataPath = Application.dataPath;

        _charactersDataPath = Path.Combine(assetDataPath, Constants.CharactersDataPath);
        _sequencesDataPath = Path.Combine(assetDataPath, Constants.SequencesDataPath);
        _stagesDataPath = Path.Combine(assetDataPath, Constants.StagesDataPath);
        _choicesDataPath = Path.Combine(assetDataPath, Constants.ChoicesDataPath);

        ResetData();
    }

    public static void SaveData()
    {
        File.WriteAllText(_charactersDataPath, _charactersData.ToString());
        File.WriteAllText(_sequencesDataPath, _sequencesData.ToString());
        File.WriteAllText(_stagesDataPath, _stagesData.ToString());
        File.WriteAllText(_choicesDataPath, _choicesData.ToString());

        _isDirty = false;
    }

    public static void ResetData()
    {
        string charactersData = File.ReadAllText(_charactersDataPath);
        string sequencesData = File.ReadAllText(_sequencesDataPath);
        string stagesData = File.ReadAllText(_stagesDataPath);
        string choicesData = File.ReadAllText(_choicesDataPath);
        
        _charactersData = ParseData(charactersData);
        _sequencesData = ParseData(sequencesData);
        _stagesData = ParseData(stagesData);
        _choicesData = ParseData(choicesData);

        _sequencesData.Sort((a, b) =>
        {
            JsonObject aJson = (JsonObject)a;
            JsonObject bJson = (JsonObject)b;

            int aSeq = aJson.GetInt("HistoryStageNumber");
            int bSeq = bJson.GetInt("HistoryStageNumber");

            return aSeq.CompareTo(bSeq);
        });

        _isDirty = false;
    }

    private static JsonArray ParseData(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return new JsonArray();
        }
        else
        {
            return SimpleJson.SimpleJson.DeserializeObject<JsonArray>(data);
        }
    } 

    public static void AddCharacter(JsonObject character)
    {
        _charactersData.Add(character);

        _isDirty = true;
    }

    public static void RemoveCharacter(string characterName)
    {
        foreach (JsonObject characterObj in _charactersData)
        {
            if (characterName.Equals((string)characterObj["Name"]))
            {
                _charactersData.Remove(characterObj);
                _isDirty = true;
                break;
            }
        }
    }

    public static void AddSequence(JsonObject sequence)
    {
        _sequencesData.Add(sequence);

        _isDirty = true;
    }

    public static void RemoveSequence(string sequenceName)
    {
        foreach (JsonObject sequenceObj in _sequencesData)
        {
            if (sequenceName.Equals((string)sequenceObj["Name"]))
            {
                _sequencesData.Remove(sequenceObj);
                _isDirty = true;
                break;
            }
        }
    }

    public static void RemoveSequence(int sequenceIndex)
    {
        _sequencesData.RemoveAt(sequenceIndex);
        _isDirty = true;
    }

    public static void AddStage(JsonObject stage)
    {
        _stagesData.Add(stage);

        _isDirty = true;
    }

    public static void RemoveStage(string stageName)
    {
        foreach (JsonObject stageObj in _stagesData)
        {
            if (stageName.Equals((string)stageObj["Name"]))
            {
                _stagesData.Remove(stageObj);
                _isDirty = true;
                break;
            }
        }
    }

    public static void AddChoice(JsonObject choice)
    {
        _choicesData.Add(choice);

        _isDirty = true;
    }

    public static void RemoveChoice(string choiceName)
    {
        foreach (JsonObject choiceObj in _choicesData)
        {
            if (choiceName.Equals((string)choiceObj["Name"]))
            {
                _choicesData.Remove(choiceObj);
                _isDirty = true;
                break;
            }
        }
    }

    public static string GetInitialSequenceName()
    {
        foreach (JsonObject seqJson in _sequencesData)
        {
            int historyStageNumber = seqJson.GetInt("HistoryStageNumber");

            if (historyStageNumber == 0)
            {
                return (string) seqJson["Name"];
            }
        }

        return null;
    }

    public static void GetInitialSequenceStartFinalStages(out string startStage, out string finalStage)
    {
        foreach (JsonObject seqJson in _sequencesData)
        {
            int historyStageNumber = seqJson.GetInt("HistoryStageNumber");

            if (historyStageNumber == 0)
            {
                startStage = (string) seqJson["StartStage"];
                finalStage = (string) seqJson["FinalStage"];
                return;
            }
        }

        startStage = null;
        finalStage = null;
    }

    public static void TryShowSavaDataDialog(Action callback)
    {
        if (_isDirty == false)
        {
            return;
        }
        else
        {
            bool saveData = EditorUtility.DisplayDialog("Unsaved data", "You have unsaved data. Do you want to save it?",
                "Save", "Don't save");

            if (saveData)
            {
                callback?.Invoke();
                
                SaveData();
            }
            else
            {
                ResetData();
            }
        }
    }

    public static void SetDirty()
    {
        _isDirty = true;
    }

    private static void RemoveUnusedStageData()
    {
        
    }

    private static void RemoveUnusedChoiceData()
    {
        
    }

    private static void RemoveUnusedSequencesData()
    {
        
    }

    //[MenuItem("Tools/Req attr value fix", false)]
    public static void ReqValueFix()
    {
        Init();

        foreach (JsonObject seqJson in _choicesData)
        {
            seqJson["ImpactValue"] = 0;
        }
        
        SaveData();
    }
}
