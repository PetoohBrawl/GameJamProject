using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class GameLoader
{
    public LocationName StartLocation { get; private set; }

    private readonly IDataStorage[] _storages = new IDataStorage[]
    {
        DialogChoicesDataStorage.Instance,
        DialogStagesDataStorage.Instance,
        DialogSequencesDataStorage.Instance,
        CharactersDataStorage.Instance,
    };

    public void LoadGame()
    {
        GameDataContainer dataContainer = Resources.Load<GameDataContainer>("GameData");

        StartLocation = dataContainer.StartLocation;

        for (int i = 0; i < _storages.Length; i++)
        {
            IDataStorage storage = _storages[i];
            string storageName = storage.GetStorageName();

            foreach (TextAsset dataAsset in dataContainer.GameDataAssets)
            {
                if (dataAsset.name.Equals(storageName))
                {
                    JsonArray dataArray = SimpleJson.SimpleJson.DeserializeObject<JsonArray>(dataAsset.text);

                    storage.Init(dataArray);
                    break;
                }
            }

            if (storage.IsInited() == false)
            {
                Debug.LogError($"No data for storage {storageName}");
            }
        }
    }
}
