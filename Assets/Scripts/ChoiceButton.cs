using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI buttonText;

    private DialogChoiceData choiceData;

    public static event Action<DialogStageData> OnChoiceMade;

    public void SetupButton(DialogChoiceData data)
    {
        choiceData = data;
        buttonText.text = choiceData.Text;
    }

    public void SetupButton(bool endOfDialog)
    {
        choiceData = null;

        if (endOfDialog)
        {
            buttonText.text = "[End dialog]";
        }
        else
        {
            buttonText.text = "[Continue]";
        }
    }

    public void OnClick()
    {
        if (choiceData == null)
        {
            OnChoiceMade?.Invoke(null);
            return;
        }

        // TODO: apply impacts

        DialogStageData stageData = GameDataStorage.Instance.GetDialogStageData(choiceData.StageName);

        OnChoiceMade?.Invoke(stageData);
    }
}
