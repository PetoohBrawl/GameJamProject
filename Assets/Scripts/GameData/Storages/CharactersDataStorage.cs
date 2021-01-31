using SimpleJson;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : IDataStorageObject
{
    public string Name { get; private set; }
    public int StartReputation { get; private set; }
    public LocationName StartLocation { get; private set; }

    private string _prefabName;
    private Dictionary<int, List<DialogSequenceData>> _dialogSequenceDatas = new Dictionary<int, List<DialogSequenceData>>();

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        StartReputation = data.GetInt("StartReputation");
        StartLocation = (LocationName)data.GetInt("StartLocation");
        _prefabName = (string)data["PrefabName"];

        JsonArray dialogSequencesNames = data.Get<JsonArray>("DialogSequences");

        foreach (string dialogSequenceName in dialogSequencesNames)
        {
            DialogSequenceData sequenceData = DialogSequencesDataStorage.Instance.GetByName(dialogSequenceName);

            if (sequenceData == null)
            {
                Debug.LogError($"DialogSequence NULL with NAME: {dialogSequenceName}, CHARACTER: {Name}");
            }

            int stageNumber = sequenceData.HistoryStageNumber;

            if (!_dialogSequenceDatas.ContainsKey(stageNumber))
            {
                _dialogSequenceDatas.Add(stageNumber, new List<DialogSequenceData>());
            }

            _dialogSequenceDatas[stageNumber].Add(sequenceData);
        }
    }

    public List<DialogSequenceData> GetHistoryStageDialogSequences(int stageNumber)
    {
        List<DialogSequenceData> dialogSequenceDatas;

        _dialogSequenceDatas.TryGetValue(stageNumber, out dialogSequenceDatas);

        return dialogSequenceDatas;
    }

    public GameObject GetPrefab()
    {
        string path = $"Prefabs/Locations/Intro/{_prefabName}";

        return Resources.Load<GameObject>(path);
    }
}

public class CharactersDataStorage : BaseDataStorage<CharacterData, CharactersDataStorage>
{
    public CharactersDataStorage() : base("Characters") { }
}
