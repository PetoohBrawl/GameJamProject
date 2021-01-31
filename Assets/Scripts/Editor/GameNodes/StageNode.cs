using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJson;

public class StageNode : Node
{
    public string StageName { get; private set; }
    private string _stagePhrase;
    private LocationName _location;
    private string _diaryString;

    public bool IsFinal { get; set; }
    
    public Node ParentNode { get; set; }

    public void Init(JsonObject json)
    {
        if (json == null)
        {
            return;
        }

        StageName = (string)json["Name"];
        _stagePhrase = (string)json["Phrase"];
        _location = (LocationName)json.GetInt(("Location"));
        _diaryString = (string)json["DiaryRecord"];
    }

    public override void Draw()
    {
        base.Draw();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Name: ", GUILayout.Width(100));
        StageName = GUILayout.TextArea(StageName, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Phrase: ", GUILayout.Width(100));
        _stagePhrase = GUILayout.TextArea(_stagePhrase, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Location: ", GUILayout.Width(100));
        _location = (LocationName)EditorGUILayout.EnumPopup(_location, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Diary string: ", GUILayout.Width(100));
        _diaryString = GUILayout.TextArea(_diaryString, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Is Final Stage: ", GUILayout.Width(100));
        IsFinal = GUILayout.Toggle(IsFinal, string.Empty);

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    public override JsonObject SerializeToJson()
    {
        JsonObject resultJson = new JsonObject()
        {
            { "Name", StageName },
            { "Phrase", _stagePhrase },
            { "Choices", null },
            { "NextStageName", null },
            { "Location", (long)_location },
            { "DiaryRecord", _diaryString },
        };

        return resultJson;
    }
}
