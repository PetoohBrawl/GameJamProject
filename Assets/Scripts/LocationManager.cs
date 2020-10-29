using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;

    public Character _characterPrefab;
    public TextMeshProUGUI _locationName;

    [SerializeField] private RectTransform _charactersParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetupLocation(GameController.Instance.CurrentLocation, null);
        }
    }

    public void SetupLocation(string locationName, Action callback)
    {
        Debug.Log("Moving to location: " + locationName);
        _locationName.text = locationName;
        // TODO вызов затемнения
        // UIManager надо будет добавить и на стартовую сцену

        // TODO как стемнеет - меняем задний фон, уничтожаем объекты старых персонажей, инстанциируем объекты персонажей новой сцены

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

        // TODO после инициализации из затемнения показываем сцену

        // TODO после выхода из затемнения вызываем колбек
        callback?.Invoke();
    }
}
