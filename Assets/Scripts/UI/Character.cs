using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string CharacterName;

    private CharacterInfo _characterInfo;

    private void Start()
    {
        if (string.IsNullOrEmpty(CharacterName) == false)
        {
            _characterInfo = new CharacterInfo(GameDataStorage.Instance.GetCharacterData(CharacterName));
        }
    }

    public void TryStartDialogSequence()
    {
        for (int i = 0; i < _characterInfo.DialogSequences.Count; i++)
        {
            if (_characterInfo.DialogSequences[i].CanStartSequence())
            {
                GameController.Instance.StartDialogSequence(_characterInfo.DialogSequences[i]);
                break;
            }
        }
    }
}
