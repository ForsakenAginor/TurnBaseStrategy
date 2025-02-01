using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class DataStorage<T>
{
    private readonly IStringSaveLoadService _saveLoadService;

    public DataStorage(string keyWord)
    {
        if (string.IsNullOrEmpty(keyWord))
            throw new ArgumentNullException(nameof(keyWord));

        _saveLoadService = new PlayerPrefsStringSaveLoadService(keyWord);
    }

    public T LoadData()
    {
        T result = default;
        string data = _saveLoadService.GetSavedInfo();
        result = JsonConvert.DeserializeObject<SerializableT>(data).Content;

        return result;
    }

    public void SaveData(T dataThatWillBeSaved)
    {
        SerializableT serializableT = new SerializableT()
        {
            Content = dataThatWillBeSaved,
        };
        string data = JsonConvert.SerializeObject(
            serializableT,
            Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            );

        Debug.Log(data);
        _saveLoadService.SaveInfo(data);
    }

    [Serializable]
    private class SerializableT
    {
        public T Content;
    }
}

[Serializable]
public class SavedData
{
    public readonly IEnumerable<Vector2Int> DiscoveredCells;
    public readonly int Wallet;
    public readonly Dictionary<Vector2Int, WalkableUnit> Units;
    public readonly Dictionary<Vector2Int, CityUnit> Cities;
    public readonly GameLevel GameLevel;

    public SavedData(IEnumerable<Vector2Int> discoveredCells, int wallet, Dictionary<Vector2Int,
        WalkableUnit> units, Dictionary<Vector2Int, CityUnit> cities, GameLevel gameLevel)
    {
        DiscoveredCells = discoveredCells != null ? discoveredCells : throw new ArgumentNullException(nameof(discoveredCells));
        Wallet = wallet >= 0 ? wallet : throw new ArgumentNullException(nameof(wallet));
        Units = units != null ? units : throw new ArgumentNullException(nameof(units));
        Cities = cities != null ? cities : throw new ArgumentNullException(nameof(cities));
        GameLevel = gameLevel;
    }
}