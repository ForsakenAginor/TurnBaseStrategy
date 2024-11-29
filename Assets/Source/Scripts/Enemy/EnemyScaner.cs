using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyScaner
{
    private readonly Dictionary<Vector2Int, Action<Vector2Int>> _enemyCities = new();
    private readonly UnitSpawner _unitSpawner;
    private readonly HexGridXZ<Unit> _grid;
    private readonly int _detectDistance = 4;
    private readonly EnemySpawnerConfiguration _configuration;

    public EnemyScaner(IEnumerable<(Vector2Int position, CitySize size)> cities, UnitSpawner unitSpawner, HexGridXZ<Unit> grid, EnemySpawnerConfiguration configuration)
    {
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));

        foreach (var city in cities)
            _enemyCities.Add(city.position, SpawnEnemies);

        _grid.GridObjectChanged += OnGridChanged;
    }

    ~EnemyScaner()
    {
        _grid.GridObjectChanged -= OnGridChanged;
    }

    private void OnGridChanged(Vector2Int coordinates)
    {
        var cell = _grid.GetGridObject(coordinates);

        if (cell == null || cell.Side == Side.Enemy)
            return;

        var cities = _enemyCities.Keys.ToArray();

        for (int i = 0; i < cities.Length; i++)
        {
            bool inDetectRange = (int)(cities[i] - coordinates).magnitude <= _detectDistance;

            if (inDetectRange && _enemyCities.ContainsKey(cities[i]))
            {
                _enemyCities[cities[i]].Invoke(cities[i]);
                _enemyCities.Remove(cities[i]);
            }
        }
    }

    private void SpawnEnemies(Vector2Int position)
    {
        var city = _grid.GetGridObject(position) as CityUnit;
        var units = _configuration.GetSpawnedPreset(city.CitySize);

        foreach (var unit in units)
            _unitSpawner.TrySpawnUnit(position, unit, Side.Enemy);
    }
}
