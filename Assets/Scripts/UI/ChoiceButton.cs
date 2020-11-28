using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private RectTransform _buttonTransform;

    private DialogChoiceData _choiceData;

    public static event Action<DialogChoiceData> OnChoiceMade;

    private const int ButtonTransformPadding = 50;
    private const int DefaultButtonWidth = 350;

    public void SetupButton(DialogChoiceData data)
    {
        _choiceData = data;
        _buttonText.text = _choiceData.Text;

        _buttonText.ForceMeshUpdate();

        float buttonWidth = Mathf.Max(_buttonText.preferredWidth + ButtonTransformPadding, DefaultButtonWidth);
        _buttonTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonWidth);

        if (_choiceData.RequiredAttribute != ImpactType.None)
        {
            int playerAttribute = PlayerProgress.Instance.GetHeroAttribute(_choiceData.RequiredAttribute);

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
            Debug.LogWarning($"Null choice data (continue or endDialog)");

            OnChoiceMade?.Invoke(null);
            return;
        }
        
        // TODO: переделать на универсальный метод применения импакта
        if (_choiceData.ApplyingImpactType == ImpactType.Reputation)
        {
            PlayerProgress.Instance.UpdateCharacterReputation(_choiceData.ImpactTargetName, _choiceData.ImpactValue);
        }
        else if (_choiceData.ApplyingImpactType != ImpactType.None)
        {
            PlayerProgress.Instance.UpdateHeroAttribute(_choiceData.ApplyingImpactType, _choiceData.ImpactValue);
        }

        Debug.LogWarning($"Trying to move to dialogStage: {_choiceData.StageName}");

        OnChoiceMade?.Invoke(_choiceData);
    }
}
