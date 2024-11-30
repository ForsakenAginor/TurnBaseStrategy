using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner
{
    private const int SpawnFrequency = 4;

    private readonly Dictionary<KeyValuePair<Vector2Int, CitySize>, Action<Vector2Int, CitySize>> _enemyCities = new();
    private readonly UnitSpawner _unitSpawner;
    private readonly EnemySpawnerConfiguration _configuration;
    private int _iterator;

    public EnemyWaveSpawner(IEnumerable<(Vector2Int position, CitySize size)> cities, UnitSpawner unitSpawner, EnemySpawnerConfiguration configuration)
    {
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));

        foreach (var city in cities)
            _enemyCities.Add(new KeyValuePair<Vector2Int, CitySize>(city.position, city.size), SpawnEnemies);
    }

    public void SpawnWave()
    {
        if (++_iterator % SpawnFrequency != 0)
            return;

        foreach (var city in _enemyCities.Keys)
        {
            var wave = _configuration.GetSpawnedPreset(city.Value);

            foreach (var unit in wave)
                _unitSpawner.TrySpawnUnit(city.Key, unit, Side.Enemy);
        }

    }

    private void SpawnEnemies(Vector2Int position, CitySize size)
    {
        var units = _configuration.GetSpawnedPreset(size);

        foreach (var unit in units)
            _unitSpawner.TrySpawnUnit(position, unit, Side.Enemy);
    }
}
