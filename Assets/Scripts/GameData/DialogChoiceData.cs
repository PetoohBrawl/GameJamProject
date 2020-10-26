using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public enum StateVariable
{
    None = -1,
    reputation = 0
}

public class DialogChoiceData
{
    public string Name { get; private set; }
    public string Text { get; private set; }
    public StateVariable StateVariable { get; private set; }
    public int ImpactValue { get; private set; }
    public string ImpactTargetName { get; private set; }
    public string StageName { get; private set; }

    public void Init(JsonObject data)
    {
        Name = (string)data["Name"];
        Text = (string)data["Text"];
        StageName = (string)data["StageName"];
        StateVariable = data.GetEnum("ImpactType", StateVariable.None);

        if (StateVariable != StateVariable.None)
        {
            ImpactValue = data.GetInt("ImpactValue");
            ImpactTargetName = (string)data["ImpactTargetName"];
        }
    }
}
