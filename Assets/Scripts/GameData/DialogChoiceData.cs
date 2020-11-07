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

public class DialogChoiceData
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

        if (string.IsNullOrEmpty(StageName))
        {
            if (GameDataStorage.Instance.GetDialogStageData(StageName) == null)
            {
                Debug.LogError($"STAGE_NAME is NULL with NAME: {StageName}, DIALOG_CHOICE: {Name}");
            }
        }

        ApplyingImpactType = data.GetEnum(data["ImpactType"].ToString(), ImpactType.None);

        if (ApplyingImpactType != ImpactType.None)
        {
            ImpactTargetName = (string)data["ImpactTargetName"];
            ImpactValue = data.GetInt("ImpactValue");
        }

        HistoryStageFinalizer = ((string)data["HistoryStageFinalizer"]).Equals("TRUE");

        RequiredAttribute = data.GetEnum(data["RequiredAttribute"].ToString(), ImpactType.None);

        if (RequiredAttribute != ImpactType.None)
        {
            RequiredAttributeValue = data.GetInt("RequiredAttributeValue");
        }

        Removable = ((string)data["Removable"]).Equals("TRUE");
    }
}
