using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class GameDataStorage
{
    private static GameDataStorage instance;
    public static GameDataStorage Instance
    {
        get
        {
            if (instance == null)
                instance = new GameDataStorage();

            return instance;
        }
    }

    public List<CharacterData> CharacterDatas = new List<CharacterData>();

    private List<DialogSequenceData> _dialogSequenceDatas = new List<DialogSequenceData>();
    private List<DialogStageData> _dialogStageDatas = new List<DialogStageData>();
    private List<DialogChoiceData> _dialogChoiceDatas = new List<DialogChoiceData>();

    public void InitStorage(GameDataContainer dataContainer)
    {
        foreach (TextAsset textAsset in dataContainer.GameDataAssets)
        {
            JsonArray dataArray = SimpleJson.SimpleJson.DeserializeObject<JsonArray>(textAsset.text);

            switch (textAsset.name)
            {
                case "Characters":
                    foreach (JsonObject data in dataArray)
                    {
                        CharacterData charData = new CharacterData();
                        charData.Init(data);

                        CharacterDatas.Add(charData);
                    }
                    break;

                case "DialogSequences":
                    foreach (JsonObject data in dataArray)
                    {
                        DialogSequenceData dialogSequenceData = new DialogSequenceData();
                        dialogSequenceData.Init(data);

                        _dialogSequenceDatas.Add(dialogSequenceData);
                    }
                    break;

                case "DialogStage":
                    foreach (JsonObject data in dataArray)
                    {
                        DialogStageData dialogStageData = new DialogStageData();
                        dialogStageData.Init(data);

                        _dialogStageDatas.Add(dialogStageData);
                    }
                    break;

                case "DialogChoice":
                    foreach (JsonObject data in dataArray)
                    {
                        DialogChoiceData dialogChoiceData = new DialogChoiceData();
                        dialogChoiceData.Init(data);

                        _dialogChoiceDatas.Add(dialogChoiceData);
                    }
                    break;
            }
        }
    }

    public DialogSequenceData GetDialogSequenceData(string name)
    {
        foreach (DialogSequenceData data in _dialogSequenceDatas)
        {
            if (data.Name.Equals(name))
                return data;
        }

        return null;
    }

    public DialogStageData GetDialogStageData(string name)
    {
        foreach (DialogStageData data in _dialogStageDatas)
        {
            if (data.Name.Equals(name))
                return data;
        }

        return null;
    }

    public DialogChoiceData GetDialogChoiceData(string name)
    {
        foreach (DialogChoiceData data in _dialogChoiceDatas)
        {
            if (data.Name.Equals(name))
                return data;
        }

        return null;
    }

    public CharacterData GetCharacterData(string name)
    {
        foreach (CharacterData data in CharacterDatas)
        {
            if (data.Name.Equals(name))
                return data;
        }

        return null;
    }
}
