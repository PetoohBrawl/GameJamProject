using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;

    private DialogChoiceData _choiceData;

    public static event Action<DialogChoiceData> OnChoiceMade;

    public void SetupButton(DialogChoiceData data)
    {
        _choiceData = data;
        _buttonText.text = _choiceData.Text;

        if (_choiceData.RequiredAttribute != ImpactType.None)
        {
            int playerAttribute = PlayerInfo.Instance.GetHeroAttribute(_choiceData.RequiredAttribute);

            bool isActive = playerAttribute >= _choiceData.RequiredAttributeValue;

            GetComponent<Button>().interactable = isActive;
        }
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
        if (_choiceData.ApplyingImpactType == ImpactType.Reputation)
        {
            GameController.Instance.UpdateCharacterReputation(_choiceData.ImpactTargetName, _choiceData.ImpactValue);
        }
        else if (_choiceData.ApplyingImpactType != ImpactType.None)
        {
            PlayerInfo.Instance.UpdateHeroAttribute(_choiceData.ApplyingImpactType, _choiceData.ImpactValue);
        }

        OnChoiceMade?.Invoke(_choiceData);
    }
}
