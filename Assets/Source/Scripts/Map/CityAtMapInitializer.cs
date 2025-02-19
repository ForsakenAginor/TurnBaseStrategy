using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SavedData;

public class CityAtMapInitializer
{
    private readonly ICityCoordinatesGetter _configuration;
    private readonly CitySpawner _citySpawner;
    private readonly GameLevel _level;

    public CityAtMapInitializer(GameLevel level, ICityCoordinatesGetter configuration, CitySpawner citySpawner)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _citySpawner = citySpawner != null ? citySpawner : throw new ArgumentNullException(nameof(citySpawner));
        _level = level;
    }

    public void SpawnCitiesFromLoadedData(SerializedPair<Vector2Int, CityData>[] cities, IEnumerable<Vector2Int> unknownCities)
    {
        foreach (var city in cities)
        {
            var data = city.Value;
            CityUpgrades upgrades = new CityUpgrades(data.IsArchersUpgraded, data.IsSpearmenUpgraded, data.IsKnightsUpgraded, data.IsMagesUpgraded, data.Income);
            //CityUpgrades upgrades = new CityUpgrades();

            if (unknownCities.Contains(city.Key))
                _citySpawner.SpawnCity(city.Key, city.Value.Size, city.Value.Side, false, upgrades, false, city.Value.Health);
            else
                _citySpawner.SpawnCity(city.Key, city.Value.Size, city.Value.Side, true, upgrades, false, city.Value.Health);
        }
    }

    public void SpawnPlayerCities()
    {
        var cities = _configuration.GetPlayerCities(_level);

        foreach (var city in cities)
            _citySpawner.SpawnCity(city.Key, city.Value, Side.Player, true);
    }

    public void SpawnEnemyCities()
    {
        var cities = _configuration.GetEnemyCities(_level);

        foreach (var city in cities)
            _citySpawner.SpawnCity(city.Key, city.Value, Side.Enemy, false);
    }
}
