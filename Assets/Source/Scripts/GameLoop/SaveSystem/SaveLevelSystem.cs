using System;
using UnityEngine;

public class SaveLevelSystem
{
    private const string Level = nameof(Level);

    public GameLevel LoadLevel()
    {
        if (PlayerPrefs.HasKey(Level) == false)
            return GameLevel.First;

        return (GameLevel)PlayerPrefs.GetInt(Level);
    }

    public void SaveLevel(GameLevel value)
    {
        PlayerPrefs.SetInt(Level, (int)value);
    }
}

public class SaveSystem
{
    private const string LevelData = nameof(LevelData);

    private readonly DataStorage<SavedData> _storage;

    private readonly ISavedFogOfWar _fogOfWar;
    private readonly ISavedUnits _units;
    private readonly ISavedCities _cities;
    private readonly ISavedWallet _wallet;
    private readonly GameLevel _level;

    public SaveSystem(ISavedFogOfWar fogOfWar, ISavedUnits units, ISavedCities cities, ISavedWallet wallet, GameLevel level)
    {
        _fogOfWar = fogOfWar != null ? fogOfWar : throw new ArgumentNullException(nameof(fogOfWar));
        _units = units != null ? units : throw new ArgumentNullException(nameof(units));
        _cities = cities != null ? cities : throw new ArgumentNullException(nameof(cities));
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _level = level;

        _storage = new(LevelData);
    }

    public void Save()
    {
        SavedData data = new SavedData(_fogOfWar.DiscoveredCells, _wallet.Amount, _units.GetInfo(), _cities.GetInfo(), _level);
        _storage.SaveData(data);
    }
}
