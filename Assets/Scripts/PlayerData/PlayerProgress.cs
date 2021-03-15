using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress
{
    private static PlayerProgress _instance;
    public static PlayerProgress Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerProgress();
            }

            return _instance;
        }
    }

    public StringBuilder HeroDiary { get; private set; } = new StringBuilder();

    private readonly List<string> _completedDialogSequenceNames = new List<string>();
    
    private CharacterInfo[] _characters;

    public void Init()
    {
        List<CharacterData> characterDatas = CharactersDataStorage.Instance.GetData();
        int charactersCount = characterDatas.Count;

        _characters = new CharacterInfo[charactersCount];

        for (int i = 0; i < charactersCount; i++)
        {
            CharacterData characterData = characterDatas[i];

            _characters[i] = new CharacterInfo(characterData);
        }
    }

    public void RecordDiary(string record)
    {
        if (string.IsNullOrEmpty(record))
        {
            return;
        }

        HeroDiary.AppendLine(record);
        HeroDiary.AppendLine();
    }

    #region HeroStats
    public void UpdateHeroAttribute(ImpactType impactType, int value)
    {
        _heroAttributes[impactType] += value;
    }

    public int GetHeroAttribute(ImpactType impactType)
    {
        return _heroAttributes[impactType];
    }
    #endregion

    #region Seqeunces
    public void CompleteDialogSequence(DialogSequenceInfo dialogSequenceInfo)
    {
        _completedDialogSequenceNames.Add(dialogSequenceInfo.DialogSequenceData.Name);
    }

    public bool IsDialogSequenceCompleted(string dialogSequenceName)
    {
        return _completedDialogSequenceNames.Contains(dialogSequenceName);
    }
    #endregion

    #region Characters
    public void UpdateCharactersState(int historyStagenumber)
    {
        foreach (CharacterInfo characterInfo in _characters)
        {
            characterInfo.SetupHistoryStageStep(historyStagenumber);
        }
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

    public List<CharacterInfo> GetCharactersInLocation(LocationName locationName)
    {
        List<CharacterInfo> characterInfos = null;

        foreach (CharacterInfo characterInfo in _characters)
        {
            if (characterInfo.Location.Equals(locationName))
            {
                if (characterInfos == null)
                {
                    characterInfos = new List<CharacterInfo>();
                }

                characterInfos.Add(characterInfo);
            }
        }

        return characterInfos;
    }
    #endregion
}
