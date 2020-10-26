using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI phraseText;
    [SerializeField]
    private RectTransform buttonsParent;
    [SerializeField]
    private ChoiceButton buttonPrefab;

    private DialogSequenceInfo currentSequenceInfo;
    private List<DialogStageData> currentStages;
    private DialogStageData currentStage;

    private void OnEnable()
    {
        ChoiceButton.OnChoiceMade += SetupView;
    }

    private void OnDisable()
    {
        ChoiceButton.OnChoiceMade -= SetupView;
    }

    public void ShowDialog(DialogSequenceInfo dialogSequenceInfo)
    {
        gameObject.SetActive(true);

        currentSequenceInfo = dialogSequenceInfo;

        DialogStageData startStage = currentSequenceInfo.DialogSequenceData.DialogStages[0];

        SetupView(startStage);
    }

    private void SetupView(DialogStageData dialogStageData)
    {
        if (dialogStageData == null)
        {
            if (string.IsNullOrEmpty(currentStage.NextStageName))
            {
                currentSequenceInfo.SetCompleted();
                gameObject.SetActive(false);
                return;
            }

            dialogStageData = GameDataStorage.Instance.GetDialogStageData(currentStage.NextStageName);
        }

        currentStage = dialogStageData;

        phraseText.text = currentStage.Phrase;
        buttonsParent.DestroyChildren();

        if (currentStage.DialogChoices.Count > 0)
        {
            foreach (DialogChoiceData choiceData in currentStage.DialogChoices)
            {
                ChoiceButton button = Instantiate(buttonPrefab, buttonsParent);
                button.SetupButton(choiceData);
            }
        }
        else
        {
            ChoiceButton button = Instantiate(buttonPrefab, buttonsParent);

            bool isEndOfDialog = string.IsNullOrEmpty(currentStage.NextStageName);
            button.SetupButton(isEndOfDialog);
        }
    }
}
