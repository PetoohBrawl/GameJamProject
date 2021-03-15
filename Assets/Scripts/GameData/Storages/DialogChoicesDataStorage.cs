using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class DialogChoiceData : IDataStorageObject
{
    public string Name { get; private set; }
    public string Text { get; private set; }
    public string StageName { get; private set; }
    public bool HistoryStageFinalizer { get; private set; }
    public bool Removable { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        Text = (string)data["Text"];
        StageName = (string)data["StageName"];

        HistoryStageFinalizer = data.GetBool("HistoryStageFinalizer");

        Removable = data.GetBool("Removable");
    }

    public void ValidateData()
    {
        if (string.IsNullOrEmpty(StageName) == false)
        {
            if (DialogStagesDataStorage.Instance.GetByName(StageName) == null)
            {
                Debug.LogError($"STAGE is NULL with NAME: {StageName}, DIALOG_CHOICE: {Name}");
            }
        }
    }
}

public class DialogChoicesDataStorage : BaseDataStorage<DialogChoiceData, DialogChoicesDataStorage>
{
    public DialogChoicesDataStorage() : base("DialogChoices") { }
}
