using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo
{
    public int ReputationValue { get; private set; }
    public LocationName Location { get; private set; }

    private DialogSequenceInfo _activeDialog;
    public DialogSequenceInfo ActiveDialog
    {
        get
        {
            if (_activeDialog == null || _activeDialog.IsCompleted)
            {
                foreach (DialogSequenceInfo dialogSequence in _dialogSequences)
                {
                    if (dialogSequence.CanStartSequence())
                    {
                        _activeDialog = dialogSequence;
                        break;
                    }
                }
            }

            return _activeDialog;
        }
    }

    public readonly CharacterData CharacterData;

    private List<DialogSequenceInfo> _dialogSequences = new List<DialogSequenceInfo>();
    private List<CharacterInfo> _characters = new List<CharacterInfo>();

    public CharacterInfo(CharacterData data)
    {
        CharacterData = data;
        ReputationValue = 0;
        Location = CharacterData.StartLocation;

        SetupHistoryStageStep(GameProgressController.Instance.CurrentHistoryStage);
    }

    public void SetupHistoryStageStep(int stageNumber)
    {
        _dialogSequences.Clear();

        List<DialogSequenceData> stageSequences = CharacterData.GetHistoryStageDialogSequences(stageNumber);

        if (stageSequences == null)
        {
            return;
        }

        foreach (DialogSequenceData dialogSequenceData in stageSequences)
        {
            _dialogSequences.Add(new DialogSequenceInfo(dialogSequenceData));
        }
    }

    public void UpdateReputation(int value)
    {
        ReputationValue += value;
    }

    public bool HasActiveSequence()
    {
        return ActiveDialog != null && (!_activeDialog.IsCompleted || _activeDialog.FinalStageInfo != null);
    }
}
