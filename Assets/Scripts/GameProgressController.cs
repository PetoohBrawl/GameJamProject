using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SimpleJson;

public class GameProgressController : MonoBehaviour
{
    public static GameProgressController Instance { get; private set; }
    
    public int CurrentHistoryStage { get; private set; } = 0;

    private int _maxHistoryStage;
    private LocationName _startLocation;
    
    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;

        GameLoader gameLoader = new GameLoader();
        gameLoader.LoadGame();
        _startLocation = gameLoader.StartLocation;

        _maxHistoryStage = DialogSequencesDataStorage.Instance.MaxHistoryStage;

        ChoiceButton.OnChoiceMade += OnChoiceMade;
    }

    public void StartGame(int historyStageNumber)
    {
        CurrentHistoryStage = historyStageNumber;

        PlayerProgress.Instance.Init();
        
        InitHistoryStage(CurrentHistoryStage);

        LocationManager.Instance.SetupLocation(_startLocation, null, () => 
        {
            DialogSequenceInfo startSequence = new DialogSequenceInfo(DialogSequencesDataStorage.Instance.StartSequenceData);
            UIManager.Instance.StartDialogSequence(startSequence);
        });

        UIManager.Instance.HideStartGameButton();
    }

    private void InitHistoryStage(int stageNumber)
    {
        Debug.LogWarning($"Initializing new history stage: {stageNumber}");

        PlayerProgress.Instance.UpdateCharactersState(stageNumber);
    }

    private void OnChoiceMade(DialogChoiceData choiceData)
    {
        if (choiceData == null || choiceData.HistoryStageFinalizer == false)
        {
            return;
        }

        CurrentHistoryStage++;

        if (CurrentHistoryStage > _maxHistoryStage)
        {
            LocationManager.Instance.ActivateShadow(null, () => 
            {
                // КОСТЫЛЬ! переделать
                AudioClip clip = Resources.Load<AudioClip>("Sounds/shot");
                MusicController.Instance.PlayOneShot(clip, () =>
                {
                    UIManager.Instance.ShowExitGameButton();
                });
            });
        }
        else
        {
            InitHistoryStage(CurrentHistoryStage);
        }
    }
}
