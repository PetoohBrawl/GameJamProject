using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class CharacterData
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
        StartLocation = data.GetEnum((string)data["StartLocation"], LocationName.Unknown);
        _prefabName = (string)data["PrefabName"];

        string[] dialogSequencesNames = ((string)data["DialogSequences"]).Split('\n');

        foreach (string dialogSequenceName in dialogSequencesNames)
        {
            DialogSequenceData sequenceData = GameDataStorage.Instance.GetDialogSequenceData(dialogSequenceName);

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
