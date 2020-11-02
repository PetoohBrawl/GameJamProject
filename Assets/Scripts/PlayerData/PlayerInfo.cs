using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    private static PlayerInfo _instance;
    public static PlayerInfo Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerInfo();
            }

            return _instance;
        }
    }

    public StringBuilder DialogHistory { get; private set; } = new StringBuilder();

    private Dictionary<ImpactType, int> _heroAttributes = new Dictionary<ImpactType, int>();
    private List<string> _completedDialogSequenceNames = new List<string>();

    public void Init()
    {
        // с 1, так как 0 атрибут - репутация
        for (int i = 1; i < (int)ImpactType.Count; i++)
        {
            _heroAttributes.Add((ImpactType)i, 0);
        }
    }

    public void UpdateHeroAttribute(ImpactType impactType, int value)
    {
        _heroAttributes[impactType] += value;
    }

    public int GetHeroAttribute(ImpactType impactType)
    {
        return _heroAttributes[impactType];
    }

    public void TryLogDialog(string record)
    {
        if (string.IsNullOrEmpty(record))
        {
            return;
        }

        DialogHistory.AppendLine(record);
        DialogHistory.AppendLine();
    }

    public void CompleteDialogSequence(DialogSequenceInfo dialogSequenceInfo)
    {
        _completedDialogSequenceNames.Add(dialogSequenceInfo.DialogSequenceData.Name);
    }

    public bool IsDialogSequenceCompleted(string dialogSequenceName)
    {
        return _completedDialogSequenceNames.Contains(dialogSequenceName);
    }
}
