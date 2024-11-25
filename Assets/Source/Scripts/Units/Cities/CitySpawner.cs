using Assets.Scripts.HexGrid;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CitySpawner : MonoBehaviour
{
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _hireInfantry;
    [SerializeField] private Button _hireSpearman;
    [SerializeField] private Button _hireArcher;
    [SerializeField] private Button _hireKnight;
    [SerializeField] private UIElement _buttonCanvas;

    private ICityPrefabGetter _configuration;
    private CitiesFactory _factory;
    private CitiesActionsManager _unitsManager;
    private HexGridXZ<Unit> _grid;
    private UnitSpawner _unitSpawner;

    public void Init(CitiesActionsManager manager, UnitSpawner unitSpawner, ICityPrefabGetter configuration, HexGridXZ<Unit> grid)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
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
                facade.UnitView.Init(unit);
                facade.Menu.Init(TryHireUnit, _upgradeButton, _hireInfantry, _hireSpearman, _hireArcher, _hireKnight, _buttonCanvas);
                _unitsManager.AddCity(unit, facade);
                break;
            default:
                break;
        }
    }

    private void TryHireUnit(UnitType type, Vector3 position)
    {
        var city = _grid.GetGridObject(position);
        Vector2Int cell = _grid.GetXZ(position);
        _unitSpawner.TrySpawnUnit(cell, type, city.Side);
    }
}