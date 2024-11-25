using Assets.Scripts.HexGrid;
using System;
using System.Linq;
using UnityEngine;

public class CitySpawner : MonoBehaviour
{
    private ICityPrefabGetter _configuration;
    private CitiesFactory _factory;
    private CitiesActionsManager _unitsManager;
    private HexGridXZ<Unit> _grid;

    public void Init(CitiesActionsManager manager, ICityPrefabGetter configuration, HexGridXZ<Unit> grid)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _factory = new CitiesFactory(configuration);
    }

    public void SpawnCity(Vector2Int position, CitySize size, Side side)
    {
        if (_grid.GetGridObject(position) != null)
            throw new Exception("Can't create city: cell is not empty");

        switch (size)
        {
            case CitySize.Village:
                var unit = _factory.CreateVillage(side);
                var facade = Instantiate(_configuration.GetPrefab(size), _grid.GetCellWorldPosition(position), Quaternion.identity);
                _unitsManager.AddCity(unit, facade);
                break;
            default:
                break;
        }
    }
}