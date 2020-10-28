using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Character : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string CharacterName;
    
    [SerializeField] private Image _glow;
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
        _glow.gameObject.SetActive(false);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _glow.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _glow.gameObject.SetActive(false);
    }
}
