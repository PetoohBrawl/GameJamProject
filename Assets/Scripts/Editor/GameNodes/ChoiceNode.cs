using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJson;

public class ChoiceNode : Node
{
    public string ChoiceName { get; private set; }
    private string _choiceText;
    private ImpactType _requiredImpact;
    private string _requiredImpactValue;
    private bool _historyStageFinalizer;
    private bool _removable;
    private ImpactType _applyingImpact;
    private string _applyingImpactValue;
    private string _applyingImpactTargetName;

    public ChoiceNode(JsonObject json) : base()
    {
        if (json == null)
        {
            return;
        }

        ChoiceName = (string)json["Name"];
        _choiceText = (string)json["Text"];
        _requiredImpact = json.GetEnum((string)json["RequiredAttribute"], ImpactType.None);
        _requiredImpactValue = (string)json["RequiredAttributeValue"];

        _historyStageFinalizer = ((string)json["HistoryStageFinalizer"]).Equals("TRUE"); // json.GetBool("HistoryStageFinalizer");
        _removable = ((string)json["Removable"]).Equals("TRUE"); // json.GetBool("Removable");

        _applyingImpact = json.GetEnum((string)json["ImpactType"], ImpactType.None);
        _applyingImpactValue = (string)json["ImpactValue"];
        _applyingImpactTargetName = (string)json["ImpactTargetName"];
    }

    public override void Draw()
    {
        base.Draw();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Name: ", GUILayout.Width(100));
        ChoiceName = GUILayout.TextArea(ChoiceName, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Text: ", GUILayout.Width(100));
        _choiceText = GUILayout.TextArea(_choiceText, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Req. Impact: ", GUILayout.Width(100));
        _requiredImpact = (ImpactType)EditorGUILayout.EnumPopup(_requiredImpact, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Value: ", GUILayout.Width(100));
        _requiredImpactValue = GUILayout.TextArea(_requiredImpactValue, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Finalizer: ", GUILayout.Width(100));
        _historyStageFinalizer = GUILayout.Toggle(_historyStageFinalizer, string.Empty);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Removable: ", GUILayout.Width(100));
        _removable = GUILayout.Toggle(_removable, string.Empty);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Apply Impact: ", GUILayout.Width(100));
        _applyingImpact = (ImpactType)EditorGUILayout.EnumPopup(_applyingImpact, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Value: ", GUILayout.Width(100));
        _applyingImpactValue = GUILayout.TextArea(_applyingImpactValue, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Target Name: ", GUILayout.Width(100));
        _applyingImpactTargetName = GUILayout.TextArea(_applyingImpactTargetName, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    public override JsonObject SerializeToJson()
    {
        JsonObject resultJson = new JsonObject()
        {
            { "Name", ChoiceName },
            { "Text", _choiceText },
            { "ImpactType", (int)_applyingImpact },
            { "ImpactValue", _applyingImpactValue },
            { "ImpactTargetName", _applyingImpactTargetName },
            { "StageName", null },
            { "HistoryStageFinalizer", _historyStageFinalizer },
            { "RequiredAttribute", _requiredImpact },
            { "RequiredAttributeValue", _requiredImpactValue },
            { "Removable", _removable },
        };

        return resultJson;
    }
}
