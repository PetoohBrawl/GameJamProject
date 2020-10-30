using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum LocationName
{
    Unknown = -1,
    Bar = 0,
    Bedroom = 1,
    Count = 2
}

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;

    public Character _characterPrefab;
    public TextMeshProUGUI _locationName;

    public LocationName CurrentLocation { get; private set; } = LocationName.Unknown; //FIXME: инициализация стартовой локации

    [SerializeField] private GameObject[] _locationButtonsContainers;
    [SerializeField] private RectTransform _charactersParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetupLocation(LocationName.Bar, null); //FIXME: при наличи сохранения надо инициализировать это место
        }
    }

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
        _locationName.text = locationName.ToString();
        // TODO вызов затемнения
        // UIManager надо будет добавить и на стартовую сцену

        // TODO как стемнеет - меняем задний фон, уничтожаем объекты старых персонажей, инстанциируем объекты персонажей новой сцены
        _charactersParent.DestroyChildren();

        List<CharacterInfo> characterInfos = GameController.Instance.GetCharacters();

        foreach (CharacterInfo characterInfo in characterInfos)
        {
            if (characterInfo.Location.Equals(locationName))
            {
                Character character = Instantiate(_characterPrefab, _charactersParent);
                character.SetCharacter(characterInfo.CharacterData.Name);
            }

            // потом доработать таблицу Characters полем-строкой с адресом префаба персонажа
        }

        for (int i = 0; i < (int)LocationName.Count; i++)
        {
            bool enableButtons = i == (int)CurrentLocation;
            _locationButtonsContainers[i].gameObject.SetActive(enableButtons);
        }

        // TODO после инициализации из затемнения показываем сцену

        // TODO после выхода из затемнения вызываем колбек
        callback?.Invoke();
    }
}
