using System;
using System.Collections.Generic;
using System.Linq;
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

    public bool CanLoad => _saveLoadService.CanLoad;

    public T LoadData()
    {
        T result = default;
        string data = _saveLoadService.GetSavedInfo();
        Debug.Log(data);
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
    public IEnumerable<Vector2Int> DiscoveredCells;
    public IEnumerable<Vector2Int> CitiesWithAvailableSpawns;
    public int Wallet;
    public int Day;
    public SerializedPair<Vector2Int, UnitData>[] Units;
    public SerializedPair<Vector2Int, CityData>[] Cities;
    public GameLevel GameLevel;

    [JsonConstructor]
    public SavedData(IEnumerable<Vector2Int> discoveredCells, int wallet, int day,
    SerializedPair<Vector2Int, UnitData>[] units,
    SerializedPair<Vector2Int, CityData>[] cities,
    GameLevel gameLevel, IEnumerable<Vector2Int> citiesWithAvailableSpawns)
    {
        DiscoveredCells = discoveredCells != null ? discoveredCells : throw new ArgumentNullException(nameof(discoveredCells));
        Wallet = wallet >= 0 ? wallet : throw new ArgumentNullException(nameof(wallet));
        Day = day >= 0 ? day : throw new ArgumentNullException(nameof(day));
        GameLevel = gameLevel;
        Units = units != null ? units : throw new ArgumentNullException(nameof(units));
        Cities = cities != null ? cities : throw new ArgumentNullException(nameof(cities));
        CitiesWithAvailableSpawns = citiesWithAvailableSpawns != null ?
            citiesWithAvailableSpawns :
            throw new ArgumentNullException(nameof(citiesWithAvailableSpawns));

    }

    public SavedData(IEnumerable<Vector2Int> discoveredCells, int wallet, int day,
        Dictionary<Vector2Int, WalkableUnit> units, Dictionary<Vector2Int, CityUnit> cities, GameLevel gameLevel,
        IEnumerable<Vector2Int> citiesWithAvailableSpawns)
    {
        DiscoveredCells = discoveredCells != null ? discoveredCells : throw new ArgumentNullException(nameof(discoveredCells));
        Wallet = wallet >= 0 ? wallet : throw new ArgumentNullException(nameof(wallet));
        Day = day >= 0 ? day : throw new ArgumentNullException(nameof(day));
        GameLevel = gameLevel;
        CitiesWithAvailableSpawns = citiesWithAvailableSpawns != null ?
            citiesWithAvailableSpawns :
            throw new ArgumentNullException(nameof(citiesWithAvailableSpawns));

        if (units == null)
            throw new ArgumentNullException(nameof(units));

        if (cities == null)
            throw new ArgumentNullException(nameof(cities));

        Units = units.
            Select(o =>
                new SerializedPair<Vector2Int, UnitData>(o.Key,
                    new UnitData(o.Value.Health, o.Value.RemainingSteps, o.Value.Side, o.Value.CanAttack, o.Value.UnitType))).
            ToArray();

        Cities = cities.
            Select(o =>
                new SerializedPair<Vector2Int, CityData>(o.Key,
                    new CityData(o.Value.Health, o.Value.Side, o.Value.CitySize,
                    o.Value.Upgrades.Income, o.Value.Upgrades.IsArcherUpgrade, o.Value.Upgrades.IsSpearmanUpgrade,
                    o.Value.Upgrades.IsKnightUpgrade, o.Value.Upgrades.IsMageUpgrade))).
            ToArray();
    }

    [Serializable]
    public struct UnitData
    {
        public readonly int Health;
        public readonly int Steps;
        public readonly Side Side;
        public readonly bool CanAttack;
        public readonly UnitType Type;

        public UnitData(int health, int steps, Side side, bool canAttack, UnitType type)
        {
            Health = health;
            Steps = steps;
            Side = side;
            CanAttack = canAttack;
            Type = type;
        }
    }

    [Serializable]
    public struct CityData
    {
        public readonly int Health;
        public readonly Side Side;
        public readonly CitySize Size;
        public readonly IncomeGrade Income;
        public readonly bool IsArchersUpgraded;
        public readonly bool IsSpearmenUpgraded;
        public readonly bool IsKnightsUpgraded;
        public readonly bool IsMagesUpgraded;

        public CityData(int health, Side side, CitySize size, IncomeGrade income,
            bool isArchersUpgraded, bool isSpearmenUpgraded, bool isKnightsUpgraded, bool isMagesUpgraded)
        {
            Health = health;
            Side = side;
            Size = size;
            Income = income;
            IsArchersUpgraded = isArchersUpgraded;
            IsSpearmenUpgraded = isSpearmenUpgraded;
            IsKnightsUpgraded = isKnightsUpgraded;
            IsMagesUpgraded = isMagesUpgraded;
        }
    }
}