using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

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
    }

    public void StartDialogSequence(DialogSequenceInfo dialogSequence)
    {
        DialogWindow.ShowDialog(dialogSequence);
    }

    public void UpdateCharacterReputation(string characterName, int reputationValue)
    {
        CharacterInfo characterInfo = GetCharacterInfo(characterName);

        if (characterInfo == null)
            return;

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
