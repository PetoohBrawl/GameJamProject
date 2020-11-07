using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    public int CurrentHistoryStage { get; private set; } = 1; // FIXME: инициализация номера этапа

    [SerializeField] private GameDataContainer _dataContainer;
    [SerializeField] private LocationManager _locationManager;
    [SerializeField] private Button _startGameButton;

    private List<CharacterInfo> _characters = new List<CharacterInfo>();
    
    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;

        GameDataStorage.Instance.InitStorage(_dataContainer);

        ChoiceButton.OnChoiceMade += OnChoiceMade;
    }

    public void OnClickStartGame()
    {
        // TODO: подгрузка сохранений или инициализация стартовых настроек
        PlayerInfo.Instance.Init();

        foreach (CharacterData characterData in GameDataStorage.Instance.CharacterDatas)
        {
            _characters.Add(new CharacterInfo(characterData));
        }
        
        InitHistoryStage(CurrentHistoryStage);
        _locationManager.SetupLocation(_dataContainer.StartLocation, null);
        _startGameButton.gameObject.SetActive(false);
    }

    private void InitHistoryStage(int stageNumber)
    {
        foreach (CharacterInfo characterInfo in _characters)
        {
            characterInfo.SetupHistoryStageStep(stageNumber);
        }
    }

    public void TryMoveNewLocation(LocationName locationName, Action callback)
    {
        _locationManager.SetupLocation(locationName, callback);
    }

    public List<CharacterInfo> GetCharacters()
    {
        return _characters;
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
