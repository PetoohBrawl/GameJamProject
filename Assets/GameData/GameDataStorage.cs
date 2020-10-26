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
    private List<DialogSequenceData> DialogSequenceDatas = new List<DialogSequenceData>();
    private List<DialogStageData> DialogStageDatas = new List<DialogStageData>();
    private List<DialogChoiceData> DialogChoiceDatas = new List<DialogChoiceData>();

    public void InitStorage(GameDataContainer dataContainer)
    {
        foreach (TextAsset textAsset in dataContainer.gameDataAssets)
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

                        DialogSequenceDatas.Add(dialogSequenceData);
                    }
                    break;

                case "DialogStage":
                    foreach (JsonObject data in dataArray)
                    {
                        DialogStageData dialogStageData = new DialogStageData();
                        dialogStageData.Init(data);

                        DialogStageDatas.Add(dialogStageData);
                    }
                    break;

                case "DialogChoice":
                    foreach (JsonObject data in dataArray)
                    {
                        DialogChoiceData dialogChoiceData = new DialogChoiceData();
                        dialogChoiceData.Init(data);

                        DialogChoiceDatas.Add(dialogChoiceData);
                    }
                    break;
            }
        }
    }

    public DialogSequenceData GetDialogSequenceData(string name)
    {
        foreach (DialogSequenceData data in DialogSequenceDatas)
        {
            if (data.Name.Equals(name))
                return data;
        }

        return null;
    }

    public DialogStageData GetDialogStageData(string name)
    {
        foreach (DialogStageData data in DialogStageDatas)
        {
            if (data.Name.Equals(name))
                return data;
        }

        return null;
    }

    public DialogChoiceData GetDialogChoiceData(string name)
    {
        foreach (DialogChoiceData data in DialogChoiceDatas)
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
