using System;
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
    public static LocationManager Instance { get; private set; }

    public LocationName CurrentLocation { get; private set; } = LocationName.Unknown;

    [SerializeField] private RectTransform _charactersParent;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _uiShadow;

    private Color _dark = new Color(0, 0, 0, 1);
    private Color _transparent = new Color(0, 0, 0, 0);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetupLocation(LocationName locationName, Action activeShadowCallback, Action deactivatedShadowCallback)
    {
        if (locationName == LocationName.Unknown || locationName.Equals(CurrentLocation))
        {
            deactivatedShadowCallback?.Invoke();
            return;
        }

        CurrentLocation = locationName;

        Sequence sequence = DOTween.Sequence();

        ActivateShadow(sequence, () => 
        {
            _charactersParent.DestroyChildren();

            List<CharacterInfo> characterInfos = PlayerProgress.Instance.GetCharactersInLocation(CurrentLocation);

            foreach (CharacterInfo characterInfo in characterInfos)
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

            activeShadowCallback?.Invoke();

            DeactivateShadow(sequence, deactivatedShadowCallback);
        });
    }

    public void ActivateShadow(Sequence sequence, Action completeCallback)
    {
        if (_uiShadow.color.Equals(_transparent))
        {
            _uiShadow.raycastTarget = true;

            if (sequence == null)
            {
                sequence = DOTween.Sequence();
            }

            sequence.Append(_uiShadow.DOColor(_dark, 2f)).OnComplete(() => 
            { 
                completeCallback?.Invoke(); 
            });
        }
        else
        {
            completeCallback?.Invoke();
        }
    }

    private void DeactivateShadow(Sequence sequence, Action callback)
    {
        if (_uiShadow.color.Equals(_dark))
        {
            sequence.Append(_uiShadow.DOColor(_transparent, 2f)).OnComplete(() =>
            {
                _uiShadow.raycastTarget = false;

                callback?.Invoke();
            });
        }
        else
        {
            callback?.Invoke();
        }

        MusicController.Instance.SwitchLocationMusic(CurrentLocation);
    }
}
