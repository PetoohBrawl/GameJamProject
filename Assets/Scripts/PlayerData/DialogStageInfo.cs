using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStageInfo
{
    public readonly DialogStageData Data;

    private List<DialogChoiceData> _activeChoices;

    public DialogStageInfo(DialogStageData data)
    {
        Data = data;

        if (Data.DialogChoices.Count > 0)
        {
            _activeChoices = new List<DialogChoiceData>();

            foreach (DialogChoiceData choiceData in Data.DialogChoices)
            {
                _activeChoices.Add(choiceData);
            }
        }
    }

    public void TryRemoveChoice(DialogChoiceData choiceData)
    {
        if (choiceData.Removable == false)
        {
            return;
        }

        _activeChoices.Remove(choiceData);
    }

    public List<DialogChoiceData> GetActiveChoices()
    {
        return _activeChoices;
    }
}
