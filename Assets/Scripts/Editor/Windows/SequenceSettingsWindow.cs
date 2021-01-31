using System;
using System.Collections.Generic;
using System.IO;
using SimpleJson;
using UnityEngine;
using UnityEditor;

public enum ReputationDirection
{
    LessThan = 0,
    MoreThan = 1
}

[Serializable]
public class RequiredSequence
{
    public string SequenceName;
    public int Index;

    public RequiredSequence(string sequenceName, int index)
    {
        SequenceName = sequenceName;
        Index = index;
    }
}

public class SequenceSettingsWindow : BaseCustomEditor
{
    private static SequenceSettingsWindow _instance;

    private int _currentSequenceIndex;
    private JsonObject _currentSequence;

    private string _sequenceName;
    private long _historyStageNumber;
    private ReputationDirection _reputationDirection;
    private long _reputationValue;
    private string _reputationCharacterName;
    [SerializeField] private List<RequiredSequence> _requiredSequences = new List<RequiredSequence>();
    
    private GenericMenu _sequencesMenu = new GenericMenu();
    private int _requiredSequenceItemIndex = -1;
    private GenericMenu _reputationCharactersMenu = new GenericMenu();
    
    public static void Open(string sequenceName, List<string> characterNames)
    {
        GameDataHelper.SetDirty();
        
        _instance = GetWindow<SequenceSettingsWindow>();

        _instance._sequenceName = sequenceName;

        for (var i = 0; i < GameDataHelper._sequencesData.Count; i++)
        {
            var seqJson = GameDataHelper._sequencesData.GetAt<JsonObject>(i);
            string seqName = (string)seqJson["Name"];

            if (sequenceName.Equals(seqName))
            {
                _instance._currentSequence = seqJson;
                _instance._currentSequenceIndex = i;

                _instance._historyStageNumber = seqJson.GetInt("HistoryStageNumber");
                _instance._reputationDirection = (ReputationDirection)seqJson.GetInt("ReputationDirection");
                _instance._reputationValue = seqJson.GetInt("ReputationValue");
                _instance._reputationCharacterName = (string)seqJson["ReputationTarget"];

                JsonArray needToCompleteSequences = seqJson.Get<JsonArray>("NeedToCompleteSequences");

                if (needToCompleteSequences == null)
                {
                    needToCompleteSequences = new JsonArray();
                }
                
                for (var index = 0; index < needToCompleteSequences.Count; index++)
                {
                    var needToCompleteSeqName = (string) needToCompleteSequences[index];

                    _instance._requiredSequences.Add(new RequiredSequence(needToCompleteSeqName, index));
                }
            }

            bool isSelectedSequence =
                _instance._requiredSequenceItemIndex >= 0 &&
                _instance._requiredSequences.Count > 0 &&
                _instance._requiredSequences[_instance._requiredSequenceItemIndex].SequenceName.Equals(seqName);

            _instance._sequencesMenu.AddItem(new GUIContent(seqName),
                isSelectedSequence,
                _instance.OnRequiredSequenceChange, seqName);
        }
        
        foreach (string charName in characterNames)
        {
            _instance._reputationCharactersMenu.AddItem(new GUIContent(charName),
                charName.Equals(_instance._reputationCharacterName), _instance.OnReputationTargetChange, charName);
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Sequence name: ");
        _sequenceName = GUILayout.TextArea(_sequenceName, GUILayout.ExpandWidth(true));
        _currentSequence["Name"] = _sequenceName;
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("History stage number: ");
        _historyStageNumber = EditorGUILayout.LongField(_historyStageNumber, GUILayout.ExpandWidth(true));
        _currentSequence["HistoryStageNumber"] = _historyStageNumber;
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Reputation Direction");
        _reputationDirection =
            (ReputationDirection)EditorGUILayout.EnumPopup(_reputationDirection, GUILayout.ExpandWidth(true));
        _currentSequence["ReputationDirection"] = (long)_reputationDirection;
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Need Reputation: ");
        _reputationValue = EditorGUILayout.LongField(_reputationValue);
        _currentSequence["ReputationValue"] = _reputationValue;
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Character name: ");

        if (GUILayout.Button(_reputationCharacterName))
        {
            _reputationCharactersMenu.ShowAsContext();
        }
        
        if (GUILayout.Button("Delete", GUILayout.Width(60)))
        {
            OnReputationTargetChange(string.Empty);
        }
        
        GUILayout.EndHorizontal();

        int sequenceToDeleteIndex = -1;
        
        foreach (RequiredSequence requiredSequence in _requiredSequences)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(requiredSequence.SequenceName))
            {
                _requiredSequenceItemIndex = requiredSequence.Index;
                _sequencesMenu.ShowAsContext();
            }

            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                _requiredSequenceItemIndex = -1;
                sequenceToDeleteIndex = requiredSequence.Index;
            }
            
            GUILayout.EndHorizontal();
        }

        if (sequenceToDeleteIndex > -1)
        {
            _requiredSequences.RemoveAt(sequenceToDeleteIndex);

            foreach (var requiredSequence in _requiredSequences)
            {
                if (requiredSequence.Index > sequenceToDeleteIndex)
                    requiredSequence.Index--;
            }
        }

        if (GUILayout.Button("Add required sequence"))
        {
            int index = _requiredSequences.Count;
            _requiredSequences.Add(new RequiredSequence("<SELECT SEQUENCE>", index));
        }

        JsonArray reqSequencesNames = new JsonArray();

        foreach (RequiredSequence reqSeq in _requiredSequences)
        {
            reqSequencesNames.Add(reqSeq.SequenceName);
        }

        _currentSequence["NeedToCompleteSequences"] = reqSequencesNames;
        
        GUILayout.Space(100);

        if (GUILayout.Button("Save"))
        {
            GameDataHelper._sequencesData[_currentSequenceIndex] = _currentSequence;
        }
        
        GUILayout.EndVertical();
    }

    public void OnRequiredSequenceChange(object newSequence)
    {
        _requiredSequences[_requiredSequenceItemIndex].SequenceName = (string)newSequence;
    }
    public void OnReputationTargetChange(object reputationTarget)
    {
        _reputationCharacterName = (string)reputationTarget;
        _currentSequence["ReputationTarget"] = _reputationCharacterName;
    }
}
