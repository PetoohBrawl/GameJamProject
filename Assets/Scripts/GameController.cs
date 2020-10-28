using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public int CurrentHistoryStage { get; private set; } = 1; // FIXME: инициализация номера этапа

    // TODO : UI надо переместить в отдельный менеджер, так как GameController не должен быть MonoBehaviour
    public DialogWindow DialogWindow;

    [SerializeField] private GameDataContainer _dataContainer;

    private List<CharacterInfo> _characters = new List<CharacterInfo>();

    private void Awake()
    {
        Instance = this;
        GameDataStorage.Instance.InitStorage(_dataContainer);

        foreach (CharacterData characterData in GameDataStorage.Instance.CharacterDatas)
        {
            _characters.Add(new CharacterInfo(characterData));
        }

        InitHistoryStage(CurrentHistoryStage);

        ChoiceButton.OnChoiceMade += OnChoiceMade;
    }

    private void OnChoiceMade(DialogChoiceData choiceData)
    {
        if (choiceData == null || choiceData.HistoryStageFinalizer == false)
        {
            return;
        }

        CurrentHistoryStage++;

        if (CurrentHistoryStage > _dataContainer.MaxHistoryStage)
        {
            // TODO: завершение игры
            Debug.Log("Game over");
        }
        else
        {
            InitHistoryStage(CurrentHistoryStage);
        }
    }

    private void InitHistoryStage(int stageNumber)
    {
        foreach (CharacterInfo characterInfo in _characters)
        {
            characterInfo.SetupHistoryStageStep(stageNumber);
        }
    }

    public void StartDialogSequence(DialogSequenceInfo dialogSequence)
    {
        DialogWindow.ShowDialog(dialogSequence);
    }

    public void UpdateCharacterReputation(string characterName, int reputationValue)
    {
        if (string.IsNullOrEmpty(characterName))
        {
            return;
        }

        CharacterInfo characterInfo = GetCharacterInfo(characterName);

        if (characterInfo == null)
        {
            return;
        }

        characterInfo.UpdateReputation(reputationValue);
    }

    public CharacterInfo GetCharacterInfo(string characterName)
    {
        foreach (CharacterInfo characterInfo in _characters)
        {
            if (characterInfo.CharacterData.Name.Equals(characterName))
            {
                return characterInfo;
            }
        }

        return null;
    }
}
