using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJson;

public class ChoiceNode : Node
{
    public string ChoiceName { get; private set; }
    private string _choiceText;
    private ImpactType _requiredImpact = ImpactType.None;
    private long _requiredImpactValue;
    private bool _historyStageFinalizer;
    private bool _removable;
    private ImpactType _applyingImpact;
    private long _applyingImpactValue;
    private string _applyingImpactTargetName;

    private GenericMenu _characterReputationTarget;

    public void Init(JsonObject json, List<string> characterNames)
    {
        _characterReputationTarget = new GenericMenu();

        foreach (string characterName in characterNames)
        {
            _characterReputationTarget.AddItem(new GUIContent(characterName), false, OnReputationTargetChange, characterName);
        }
        
        if (json == null)
        {
            return;
        }

        ChoiceName = (string)json["Name"];
        _choiceText = (string)json["Text"];
        _requiredImpact = (ImpactType)json.GetInt("RequiredAttribute");
        _requiredImpactValue = json.GetInt("RequiredAttributeValue");

        _historyStageFinalizer = json.GetBool("HistoryStageFinalizer");
        _removable = json.GetBool("Removable");

        _applyingImpact = (ImpactType)json.GetInt("ImpactType");
        _applyingImpactValue = json.GetInt("ImpactValue");
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
        _requiredImpactValue = EditorGUILayout.LongField(_requiredImpactValue, GUILayout.ExpandWidth(true));

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
        _applyingImpactValue = EditorGUILayout.LongField(_applyingImpactValue, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Target Name: ", GUILayout.Width(100));

        if (GUILayout.Button(_applyingImpactTargetName))
        {
            _characterReputationTarget.ShowAsContext();
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void OnReputationTargetChange(object characterName)
    {
        _applyingImpactTargetName = (string) characterName;
    }

    public override JsonObject SerializeToJson()
    {
        JsonObject resultJson = new JsonObject()
        {
            { "Name", ChoiceName },
            { "Text", _choiceText },
            { "ImpactType", (long)_applyingImpact },
            { "ImpactValue", _applyingImpactValue },
            { "ImpactTargetName", _applyingImpactTargetName },
            { "StageName", null },
            { "HistoryStageFinalizer", _historyStageFinalizer },
            { "RequiredAttribute", (long)_requiredImpact },
            { "RequiredAttributeValue", _requiredImpactValue },
            { "Removable", _removable },
        };

        return resultJson;
    }
}
