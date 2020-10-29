﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string CharacterName;
    
    [SerializeField] private Image _glow;
    [SerializeField] private TextMeshProUGUI _characterName;
    private CharacterInfo _characterInfo;

    private void Start()
    {
        _characterName.text = CharacterName;
        _characterInfo = GameController.Instance.GetCharacterInfo(CharacterName);
        _glow.gameObject.SetActive(false);
    }

    public void SetCharacter(string name)
    {
        CharacterName = name;
    }

    public void TryStartDialogSequence()
    {
        DialogSequenceInfo activeDialog = _characterInfo.ActiveDialog;

        // TODO: обработать ситуацию, если игроку недоступен ни один из вариантов диалога с персонажем
        if (activeDialog == null)
        {
            
            return;
        }

        UIManager.Instance.StartDialogSequence(activeDialog);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _glow.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _glow.gameObject.SetActive(false);
    }
}
