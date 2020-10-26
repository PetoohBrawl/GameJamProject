using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string _characterName;

    private CharacterInfo characterInfo;

    private void Start()
    {
        if (string.IsNullOrEmpty(_characterName) == false)
        {
            characterInfo = new CharacterInfo(GameDataStorage.Instance.GetCharacterData(_characterName));
        }
    }

    public void TryStartDialogSequence()
    {
        for (int i = 0; i < characterInfo.dialogSequences.Count; i++)
        {
            if (characterInfo.dialogSequences[i].CanStartSequence())
            {
                GameController.Instance.StartDialogSequence(characterInfo.dialogSequences[i]);
                break;
            }
        }
    }
}
