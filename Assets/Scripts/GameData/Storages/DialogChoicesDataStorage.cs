using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public enum ImpactType
{
    None = -1,
    Reputation = 0,
    Composure = 1,

    Count
}

public class DialogChoiceData : IDataStorageObject
{
    public string Name { get; private set; }
    public string Text { get; private set; }
    public string StageName { get; private set; }
    public bool HistoryStageFinalizer { get; private set; }
    public ImpactType RequiredAttribute { get; private set; }
    public int RequiredAttributeValue { get; private set; }
    public bool Removable { get; private set; }

    // TODO : объединить в отдельный класс воздействий (какой-нибудь ImpactInfo), который по вызову метода Apply будет применять импакты
    public ImpactType ApplyingImpactType { get; private set; }
    public int ImpactValue { get; private set; }
    public string ImpactTargetName { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        Text = (string)data["Text"];
        StageName = (string)data["StageName"];

        ApplyingImpactType = (ImpactType)data.GetInt("ImpactType");

        if (ApplyingImpactType != ImpactType.None)
        {
            ImpactTargetName = (string)data["ImpactTargetName"];
            ImpactValue = data.GetInt("ImpactValue");
        }

        HistoryStageFinalizer = data.GetBool("HistoryStageFinalizer");
        
        RequiredAttribute = (ImpactType)data.GetInt("RequiredAttribute");

        if (RequiredAttribute != ImpactType.None)
        {
            RequiredAttributeValue = data.GetInt("RequiredAttributeValue");
        }

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
