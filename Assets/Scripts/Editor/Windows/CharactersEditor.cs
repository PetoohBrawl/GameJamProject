using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJson;

public class CharactersEditor : BaseCustomEditor
{
    private static CharactersEditor _instance;

    private int _currentSelectedCharacter;
    private readonly List<string> _characterNames = new List<string>();
    private string _newCharacterName;
    private string _newSequenceName;

    [MenuItem("Tools/Open Dialogues Editor")]
    public static void Open()
    {
        GameDataHelper.Init();
        
        _instance = GetWindow<CharactersEditor>();

        foreach (JsonObject characterJson in GameDataHelper._charactersData)
        {
            _instance._characterNames.Add((string)characterJson["Name"]);
        }

        _instance._currentSelectedCharacter = 0;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        // 1: Поле имени нового персонажа, кнопка создания нового персонажа.
        GUILayout.BeginVertical();

        _newCharacterName = GUILayout.TextArea(_newCharacterName);

        if (GUILayout.Button("Add Character"))
        {
            if (string.IsNullOrEmpty(_newCharacterName) == false)
            {
                JsonObject newCharacterObject = new JsonObject()
                {
                    { "Name", _newCharacterName },
                    { "StartReputation", 0L },
                    { "DialogSequences", new JsonArray() },
                    { "StartLocation", (long)LocationName.Unknown },
                    { "PrefabName", null }
                };

                _characterNames.Add(_newCharacterName);
                GameDataHelper.AddCharacter(newCharacterObject);

                _currentSelectedCharacter = _characterNames.Count - 1;
            }
        }

        GUILayout.Space(100);

        if (GUILayout.Button("Save character"))
        {
            GameDataHelper.SaveData();
        }
        
        GUILayout.Space(200);

        string initialStageName = GameDataHelper.GetInitialSequenceName();

        if (string.IsNullOrEmpty(initialStageName) == false)
        {
            if (GUILayout.Button("Setup initial sequence", GUILayout.Width(200), GUILayout.Height(100)))
            {
                GameDataHelper.GetInitialSequenceStartFinalStages(out string startStageName, out string finalStageName);

                NodeBasedEditor.OpenWindow(startStageName, finalStageName, initialStageName, _characterNames);
            }
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete initial stage"))
            {
                GameDataHelper.RemoveSequence(initialStageName);
            }            
            
            if (GUILayout.Button("Settings"))
            {
                SequenceSettingsWindow.Open(initialStageName, _characterNames);
            }
            
            GUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Create initial stage", GUILayout.Width(200), GUILayout.Height(100)))
            {
                JsonObject initialStageObj = new JsonObject()
                {
                    { "Name", "initial_stage" },
                    { "StartStage", null },
                    { "FinalStage", null },
                    { "HistoryStageNumber", 0L },
                    { "ReputationDirection", 0L },
                    { "ReputationValue", 0L },
                    { "ReputationTarget", string.Empty },
                    { "NeedCompleteSequences", null }
                };
                
                GameDataHelper.AddSequence(initialStageObj);
                
                SequenceSettingsWindow.Open("initial_stage", _characterNames);
            }
        }

        GUILayout.EndVertical();

        if (_characterNames.Count == 0)
        {
            return;
        }

        // 2: Выпадающий список персонажей и настройки текущего персонажа: имя, стартовая репутация, локация, имя префаба. 
        // Под настройками кнопка добавления секвенса. Внизу кнопка удаления персонажа.
        GUILayout.BeginVertical();

        _currentSelectedCharacter = EditorGUILayout.Popup(_currentSelectedCharacter, _characterNames.ToArray());

        JsonObject characterCache = GameDataHelper._charactersData.GetAt<JsonObject>(_currentSelectedCharacter);
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Start reputation: ");

        characterCache["StartReputation"] = (long)EditorGUILayout.IntField(characterCache.GetInt("StartReputation"));
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Start location: ");
        
        long locationInt = characterCache.GetInt("StartLocation");
        LocationName startLocation = (LocationName)locationInt;
        startLocation = (LocationName)EditorGUILayout.EnumPopup(startLocation);
        characterCache["StartLocation"] = (long)startLocation;
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();

        GUILayout.Label("Prefab name: ");
        
        characterCache["PrefabName"] = GUILayout.TextArea((string)characterCache["PrefabName"]);
        
        GUILayout.EndHorizontal();

        GUILayout.Space(100);

        _newSequenceName = GUILayout.TextArea(_newSequenceName);

        if (GUILayout.Button("Add Sequence"))
        {
            if (string.IsNullOrEmpty(_newSequenceName) == false)
            {
                JsonObject newSeqObj = new JsonObject()
                {
                    { "Name", _newSequenceName },
                    { "StartStage", null },
                    { "FinalStage", null },
                    { "HistoryStageNumber", -1L },
                    { "ReputationDirection", 1L },
                    { "ReputationValue", 0L },
                    { "ReputationTarget", string.Empty },
                    { "NeedCompleteSequences", null }
                };

                characterCache.Get<JsonArray>("DialogSequences").Add(_newSequenceName);
                GameDataHelper.AddSequence(newSeqObj);
            }
        }

        GameDataHelper._charactersData[_currentSelectedCharacter] = characterCache;

        GUILayout.Space(100);

        if (GUILayout.Button("Delete Character"))
        {
            GameDataHelper.RemoveCharacter(_characterNames[_currentSelectedCharacter]);
            _characterNames.RemoveAt(_currentSelectedCharacter);
            _currentSelectedCharacter = 0;
        }

        GUILayout.EndVertical();

        // 3: Кнопки секвенсов + рядом кнопка удаления секвенса
        GUILayout.BeginVertical();

        JsonArray characterSequences = GameDataHelper._charactersData.GetAt<JsonObject>(_currentSelectedCharacter).Get<JsonArray>("DialogSequences");

        int sequenceToRemoveIndex = -1;
        
        for (int i = 0; i < GameDataHelper._sequencesData.Count; i++)
        {
            GUILayout.BeginHorizontal();
            
            string seqName = (string)GameDataHelper._sequencesData.GetAt<JsonObject>(i)["Name"];

            if (characterSequences.Contains(seqName))
            {
                if (GUILayout.Button($"{seqName}"))
                {
                    string startStageName = (string)GameDataHelper._sequencesData.GetAt<JsonObject>(i)["StartStage"];
                    string finalStageName = (string)GameDataHelper._sequencesData.GetAt<JsonObject>(i)["FinalStage"];

                    NodeBasedEditor.OpenWindow(startStageName, finalStageName, seqName, _characterNames);
                }

                if (GUILayout.Button("Settings", GUILayout.Width(100f)))
                {
                    SequenceSettingsWindow.Open(seqName, _characterNames);
                }

                if (GUILayout.Button("Delete", GUILayout.Width(100f)))
                {
                    sequenceToRemoveIndex = i;
                }
            }
            
            GUILayout.EndHorizontal();
        }

        if (sequenceToRemoveIndex > -1)
        {
            GameDataHelper.RemoveSequence(sequenceToRemoveIndex);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}
