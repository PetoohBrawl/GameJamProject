using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SimpleJson;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    public int CurrentHistoryStage { get; private set; } = 0; // FIXME: инициализация номера этапа

    [SerializeField] private GameDataContainer _dataContainer;
    [SerializeField] private LocationManager _locationManager;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _exitGameButton;

    private List<CharacterInfo> _characters = new List<CharacterInfo>();
    private int _maxHistoryStage;

    private IDataStorage[] _storages = new IDataStorage[]
    {
        DialogChoicesDataStorage.Instance,
        DialogStagesDataStorage.Instance,
        DialogSequencesDataStorage.Instance,
        CharactersDataStorage.Instance,
    };
    
    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;

        for (int i = 0; i < _storages.Length; i++)
        {
            IDataStorage storage = _storages[i];
            string storageName = storage.GetStorageName();

            foreach (TextAsset dataAsset in _dataContainer.GameDataAssets)
            {
                if (dataAsset.name.Equals(storageName))
                {
                    JsonArray dataArray = SimpleJson.SimpleJson.DeserializeObject<JsonArray>(dataAsset.text);

                    storage.Init(dataArray);
                    break;
                }
            }

            if (storage.IsInited() == false)
            {
                Debug.LogError($"No data for storage {storageName}");
            }
        }

        _maxHistoryStage = DialogSequencesDataStorage.Instance.MaxHistoryStage;

        ChoiceButton.OnChoiceMade += OnChoiceMade;
    }

    public void OnClickStartGame()
    {
        // TODO: подгрузка сохранений или инициализация стартовых настроек
        PlayerInfo.Instance.Init();

        List<CharacterData> characterDatas = CharactersDataStorage.Instance.GetData();

        foreach (CharacterData characterData in characterDatas)
        {
            _characters.Add(new CharacterInfo(characterData));
        }
        
        InitHistoryStage(CurrentHistoryStage);
        _locationManager.SetupLocation(_dataContainer.StartLocation, null, () => 
        {
            DialogSequenceInfo startSequence = new DialogSequenceInfo(DialogSequencesDataStorage.Instance.StartSequenceData);
            UIManager.Instance.StartDialogSequence(startSequence);
        });

        _startGameButton.gameObject.SetActive(false);
    }

    private void InitHistoryStage(int stageNumber)
    {
        Debug.LogWarning($"Initializing new history stage: {stageNumber}");

        foreach (CharacterInfo characterInfo in _characters)
        {
            characterInfo.SetupHistoryStageStep(stageNumber);
        }
    }

    public void TryMoveNewLocation(LocationName locationName, Action activeShadowCallback, Action deactivatedShadowCallback)
    {
        _locationManager.SetupLocation(locationName, activeShadowCallback, deactivatedShadowCallback);
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

        if (CurrentHistoryStage > _maxHistoryStage)
        {
            Sequence sequence = DOTween.Sequence();

            _locationManager.ActivateShadow(sequence, () => 
            {
                // КОСТЫЛЬ! переделать
                AudioClip clip = Resources.Load<AudioClip>("Sounds/shot");
                StartCoroutine(MusicController.Instance.PlayOneShot(clip, () =>
                {
                    _exitGameButton.gameObject.SetActive(true);
                }));
            });
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

    public void OnClickExitGame()
    {
        Application.Quit();
    }
}
