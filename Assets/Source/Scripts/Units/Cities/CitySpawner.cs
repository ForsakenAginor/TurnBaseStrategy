using Assets.Scripts.HexGrid;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CitySpawner : MonoBehaviour, IUnitSpawner
{
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _hireInfantry;
    [SerializeField] private Button _hireSpearman;
    [SerializeField] private Button _hireArcher;
    [SerializeField] private Button _hireKnight;
    [SerializeField] private UIElement _buttonCanvas;
    [SerializeField] private TMP_Text _upgradeCostLabel;
    [SerializeField] private UIElement _upgradePanel;

    private CitiesConfiguration _configuration;
    private CitiesFactory _factory;
    private CitiesActionsManager _unitsManager;
    private HexGridXZ<Unit> _grid;
    private UnitSpawner _unitSpawner;
    private Resource _wallet;

    public event Action<Unit> UnitSpawned;
    public Action<AudioSource> AudioSourceCallback;

    private void OnDestroy()
    {
        _unitsManager.CityCaptured -= OnCityCaptured;
    }

    public void Init(CitiesActionsManager manager, UnitSpawner unitSpawner, Resource wallet,
        CitiesConfiguration configuration, HexGridXZ<Unit> grid, Action<AudioSource> callback)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        AudioSourceCallback = callback != null ? callback : throw new ArgumentNullException(nameof(callback));
        _factory = new CitiesFactory(configuration);

        _unitsManager.CityCaptured += OnCityCaptured;
    }

    public void SpawnCity(Vector2Int position, CitySize size, Side side)
    {
        if (_grid.GetGridObject(position) != null)
            throw new Exception("Can't create city: cell is not empty");

        var unit = _factory.Create(size, side);
        var facadePrefab = side == Side.Player ? _configuration.GetPlayerPrefab(size) : _configuration.GetEnemyPrefab(size);
        var facade = Instantiate(facadePrefab, _grid.GetCellWorldPosition(position), Quaternion.identity);
        facade.UnitView.Init(unit, AudioSourceCallback);
        facade.Menu.Init(TryHireUnit, TryUpgradeCity,
            _upgradeButton, _hireInfantry, _hireSpearman, _hireArcher, _hireKnight, _buttonCanvas,
            _upgradeCostLabel, _configuration.GetUpgradeCost(size), _upgradePanel);
        _unitsManager.AddCity(unit, facade);

        //todo: subscribe to city died event, for spawn opposite side village on died
        UnitSpawned?.Invoke(unit);
    }

    private void OnCityCaptured(Vector2Int cell, Side side)
    {
        SpawnCity(cell, CitySize.Village, side);
    }

    private bool TryUpgradeCity(Vector3 position)
    {
        var city = _grid.GetGridObject(position) as CityUnit;
        Side side = city.Side;
        CitySize size = city.CitySize;
        Vector2Int cell = _grid.GetXZ(position);

        if ((int)size == Enum.GetValues(typeof(CitySize)).Length - 1)
            return false;

        int cost = _configuration.GetUpgradeCost(size);

        if (side == Side.Player)
            if (_wallet.TrySpent(cost) == false)
                return false;

        _unitsManager.RemoveCity(city);
        size++;
        SpawnCity(cell, size, side);
        _buttonCanvas.Disable();
        return true;
    }

    private bool TryHireUnit(UnitType type, Vector3 position)
    {
        var city = _grid.GetGridObject(position);
        Vector2Int cell = _grid.GetXZ(position);
        return _unitSpawner.TrySpawnUnit(cell, type, city.Side);
    }
}
