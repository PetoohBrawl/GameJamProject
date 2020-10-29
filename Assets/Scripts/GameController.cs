﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    public int CurrentHistoryStage { get; private set; } = 1; // FIXME: инициализация номера этапа

    private string _currentLocation = "Bar"; //FIXME: инициализация стартовой локации
    private GameDataContainer _dataContainer;
    private List<CharacterInfo> _characters = new List<CharacterInfo>();
    
    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;

        _dataContainer = Resources.Load<GameDataContainer>("GameData");
        GameDataStorage.Instance.InitStorage(_dataContainer);

        foreach (CharacterData characterData in GameDataStorage.Instance.CharacterDatas)
        {
            _characters.Add(new CharacterInfo(characterData));
        }

        ChoiceButton.OnChoiceMade += OnChoiceMade;
    }

    public void OnClickStartGame()
    {
        InitHistoryStage(CurrentHistoryStage);

        SceneManager.LoadScene("GameScene");
    }

    private void InitHistoryStage(int stageNumber)
    {
        foreach (CharacterInfo characterInfo in _characters)
        {
            characterInfo.SetupHistoryStageStep(stageNumber);
        }
    }

    public void TryMoveNewLocation(string locationName, Action callback)
    {
        if (string.IsNullOrEmpty(locationName) || _currentLocation.Equals(locationName))
        {
            callback.Invoke();
        }
        else
        {
            // TODO: реинициализация сцены через затемнение
            Debug.Log("Moving to location: " + locationName);
            callback.Invoke();
        }
    }

    private void SetupLocation(string locationName)
    {
        // TODO: смена локации
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
