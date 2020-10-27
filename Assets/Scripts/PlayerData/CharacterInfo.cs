using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo
{
    public int ReputationValue { get; private set; }

    private DialogSequenceInfo _activeDialog;
    public DialogSequenceInfo ActiveDialog
    {
        get
        {
            if (_activeDialog == null)
            {
                foreach (DialogSequenceInfo dialogSequence in _dialogSequences)
                {
                    if (dialogSequence.CanStartSequence())
                    {
                        // Если для конкретной части игры уже выбран диалог с персонажем, то имеет смысл очистить кэш sequence'ов ?
                        //_dialogSequences.Clear();

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

    public static event Action<string> OnCharacterInfoUpdated;

    public CharacterInfo(CharacterData data)
    {
        CharacterData = data;
        ReputationValue = 0;

        SetupHistoryStageStep(GameController.Instance.CurrentHistoryStage);
    }

    public void SetupHistoryStageStep(int stageNumber)
    {
        _dialogSequences.Clear();

        List<DialogSequenceData> stageSequences = CharacterData.GetHistoryStageDialogSequences(stageNumber);

        foreach (DialogSequenceData dialogSequenceData in stageSequences)
        {
            _dialogSequences.Add(new DialogSequenceInfo(dialogSequenceData));
        }

        OnCharacterInfoUpdated?.Invoke(CharacterData.Name);
    }

    public void UpdateReputation(int value)
    {
        ReputationValue += value;
    }
}
