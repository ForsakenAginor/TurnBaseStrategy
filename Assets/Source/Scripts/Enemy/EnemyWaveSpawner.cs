using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner
{
    private readonly Dictionary<CityUnit, KeyValuePair<Vector2Int, int>> _cities = new();
    private readonly UnitSpawner _unitSpawner;
    private readonly EnemyWaveConfiguration _configuration;
    private int _iterator;

    public EnemyWaveSpawner(IEnumerable<(Vector2Int position, CitySize size, CityUnit unit)> cities, UnitSpawner unitSpawner,
        EnemyWaveConfiguration configuration)
    {
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));

        foreach (var city in cities)
        {
            _cities.Add(city.unit, new KeyValuePair<Vector2Int, int>(city.position, _configuration.GetSpawnFrequency(city.size)));
            city.unit.Captured += OnCityCaptured;
        }
    }

    ~EnemyWaveSpawner()
    {
        foreach (var city in _cities.Keys)
            city.Captured -= OnCityCaptured;
    }

    public void SpawnWave()
    {
        int iterator = ++_iterator;

        foreach (var city in _cities.Keys)
        {
            if (iterator % _cities[city].Value != 0)
                return;

            var wave = _configuration.GetSpawnedPreset(city.CitySize);

            foreach (var unit in wave)
                _unitSpawner.TrySpawnUnit(_cities[city].Key, unit, Side.Enemy);
        }
    }

    private void OnCityCaptured(Unit unit)
    {
        unit.Captured -= OnCityCaptured;
        _cities.Remove(unit as CityUnit);
    }
}
