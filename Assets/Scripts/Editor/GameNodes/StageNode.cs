using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJson;

public class StageNode : Node
{
    private string _stageName;
    private string _stagePhrase;
    private LocationName _location;
    private string _diaryString;

    public StageNode(JsonObject json) : base()
    {
        if (json == null)
        {
            return;
        }

        _stageName = (string)json["Name"];
        _stagePhrase = (string)json["Phrase"];
        _location = json.GetEnum((string)json["Location"], LocationName.Unknown);
        _diaryString = (string)json["DiaryRecord"];
    }

    public override void Draw()
    {
        base.Draw();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Name: ", GUILayout.Width(100));
        _stageName = GUILayout.TextArea(_stageName, GUILayout.ExpandWidth(true));

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

        GUILayout.EndVertical();
    }
}
