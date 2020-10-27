using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public enum ImpactType
{
    None = -1,
    Reputation = 0
}

public class DialogChoiceData
{
    public string Name { get; private set; }
    public string Text { get; private set; }
    public string StageName { get; private set; }
    public bool HistoryStageFinalizer { get; private set; }

    // TODO : объединить в отдельный класс воздействий (какой-нибудь ImpactInfo), который по вызову метода Apply будет применять импакты
    public int ImpactValue { get; private set; }
    public string ImpactTargetName { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        Text = (string)data["Text"];
        StageName = (string)data["StageName"];
        ImpactTargetName = (string)data["ImpactTargetName"];

        if (!string.IsNullOrEmpty(ImpactTargetName))
        {
            ImpactValue = data.GetInt("ImpactValue");
        }

        HistoryStageFinalizer = ((string)data["HistoryStageFinalizer"]).Equals("TRUE");
    }
}
