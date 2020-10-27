using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string CharacterName;

    private CharacterInfo _characterInfo;

    private void OnEnable()
    {
        CharacterInfo.OnCharacterInfoUpdated += OnCharacterInfoUpdated;
    }

    private void OnDisable()
    {
        CharacterInfo.OnCharacterInfoUpdated -= OnCharacterInfoUpdated;
    }

    private void Start()
    {
        _characterInfo = GameController.Instance.GetCharacterInfo(CharacterName);
    }

    private void OnCharacterInfoUpdated(string name)
    {
        if (!CharacterName.Equals(name))
        {
            return;
        }

        if (string.IsNullOrEmpty(CharacterName) == false)
        {
            _characterInfo = GameController.Instance.GetCharacterInfo(CharacterName);
        }
    }

    public void TryStartDialogSequence()
    {
        DialogSequenceInfo activeDialog = _characterInfo.ActiveDialog;

        // TODO: обработать ситуацию, если игроку недоступен ни один из вариантов диалога с персонажем
        if (activeDialog == null)
        {
            
            return;
        }

        GameController.Instance.StartDialogSequence(activeDialog);
    }
}
