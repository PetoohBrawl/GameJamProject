using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;

    private DialogChoiceData _choiceData;

    public static event Action<DialogChoiceData> OnChoiceMade;

    public void SetupButton(DialogChoiceData data)
    {
        _choiceData = data;
        _buttonText.text = _choiceData.Text;
    }

    public void SetupButton(bool endOfDialog)
    {
        _choiceData = null;

        if (endOfDialog)
        {
            _buttonText.text = "[End dialog]";
        }
        else
        {
            _buttonText.text = "[Continue]";
        }
    }

    public void OnClick()
    {
        if (_choiceData == null)
        {
            OnChoiceMade?.Invoke(null);
            return;
        }
        
        // TODO: переделать на универсальный метод применения импакта
        GameController.Instance.UpdateCharacterReputation(_choiceData.ImpactTargetName, _choiceData.ImpactValue);

        OnChoiceMade?.Invoke(_choiceData);
    }
}
