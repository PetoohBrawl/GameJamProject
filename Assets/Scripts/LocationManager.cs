﻿using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public enum LocationName
{
    Unknown = -1,
    Intro = 0,
    Bedroom = 1,
    Count = 2
}

public class LocationManager : MonoBehaviour
{
    public LocationName CurrentLocation { get; private set; } = LocationName.Unknown; //FIXME: инициализация стартовой локации

    [SerializeField] private RectTransform _charactersParent;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _uiShadow;

    private Color _dark = new Color(0, 0, 0, 1);
    private Color _transparent = new Color(0, 0, 0, 0);

    public void MoveLocation(int locationIndex)
    {
        SetupLocation((LocationName)locationIndex, null);
    }

    public void SetupLocation(LocationName locationName, Action callback)
    {
        if (locationName == LocationName.Unknown || locationName.Equals(CurrentLocation))
        {
            callback?.Invoke();
            return;
        }

        Debug.Log("Moving to location: " + locationName.ToString());
        CurrentLocation = locationName;

        Sequence sequence = DOTween.Sequence();

        ActivateShadow(sequence, () => 
        {
            _charactersParent.DestroyChildren();

            List<CharacterInfo> characterInfos = GameController.Instance.GetCharacters();

            foreach (CharacterInfo characterInfo in characterInfos)
            {
                if (characterInfo.Location.Equals(locationName))
                {
                    GameObject characterPrefab = Instantiate(characterInfo.CharacterData.GetPrefab(), _charactersParent);

                    // костыль, чтобы корректно отображался столик в Intro Локации
                    if (locationName == LocationName.Intro && characterPrefab.name.Contains("Table"))
                    {
                        characterPrefab.transform.SetAsFirstSibling();
                    }

                    Character character = characterPrefab.GetComponent<Character>();
                    character.SetCharacter(characterInfo.CharacterData.Name);
                }
            }
        });

        DeactivateShadow(sequence);

        callback?.Invoke();
    }

    private void ActivateShadow(Sequence sequence, Action completeCallback)
    {
        if (_uiShadow.color.Equals(_transparent))
        {
            sequence.Append(_uiShadow.DOColor(_dark, 2f)).OnComplete(() => 
            { 
                completeCallback?.Invoke(); 
            });

            _uiShadow.raycastTarget = true;
        }
        else
        {
            completeCallback?.Invoke();
        }
    }

    private void DeactivateShadow(Sequence sequence)
    {
        if (_uiShadow.color.Equals(_dark))
        {
            sequence.Append(_uiShadow.DOColor(_transparent, 2f)).OnComplete(() =>
            {
                _uiShadow.raycastTarget = false;
            });
        }
    }
}
