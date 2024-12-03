using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner
{
    private readonly Dictionary<KeyValuePair<Vector2Int, CitySize>, int> _enemyCities = new();
    private readonly UnitSpawner _unitSpawner;
    private readonly EnemyWaveConfiguration _configuration;
    private int _iterator;

    public EnemyWaveSpawner(IEnumerable<(Vector2Int position, CitySize size)> cities, UnitSpawner unitSpawner,
        EnemyWaveConfiguration configuration)
    {
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));

        foreach (var city in cities)
            _enemyCities.Add(new KeyValuePair<Vector2Int, CitySize>(city.position, city.size),
                _configuration.GetSpawnFrequency(city.size));
    }

    public void SpawnWave()
    {
        foreach (var city in _enemyCities.Keys)
        {
            if (++_iterator % _enemyCities[city] != 0)
                return;

            var wave = _configuration.GetSpawnedPreset(city.Value);

            foreach (var unit in wave)
                _unitSpawner.TrySpawnUnit(city.Key, unit, Side.Enemy);
        }
    }
}
