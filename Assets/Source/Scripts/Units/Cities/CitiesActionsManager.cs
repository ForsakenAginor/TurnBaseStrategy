using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CitiesActionsManager : ICitiesGetter, ISavedCities
{
    private readonly Dictionary<CityUnit, ICityFacade> _cities = new Dictionary<CityUnit, ICityFacade>();
    private readonly IEnumerable<NewInputSorter> _inputSorters;
    private readonly HexGridXZ<Unit> _grid;

    private ISwitchableElement _selectedUnit;
    private ISwitchableElement _selectedUnitMenu;

    /// <summary>
    /// Normal game constructor
    /// </summary>
    /// <param name="inputSorter"></param>
    /// <param name="grid"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CitiesActionsManager(NewInputSorter inputSorter, HexGridXZ<Unit> grid)
    {
        _inputSorters = inputSorter != null ? new List<NewInputSorter>() { inputSorter } : throw new ArgumentNullException(nameof(inputSorter));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));

        foreach (var input in _inputSorters)
        {
            input.MovableUnitSelected += OnUnitSelected;
            input.FriendlyCitySelected += OnCitySelected;
            input.EnemySelected += OnCitySelected;
            input.BecomeInactive += OnDeselect;
        }
    }

    /// <summary>
    /// HotSit constructor
    /// </summary>
    /// <param name="inputSorters"></param>
    /// <param name="grid"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CitiesActionsManager(IEnumerable<NewInputSorter> inputSorters, HexGridXZ<Unit> grid)
    {
        _inputSorters = inputSorters != null ? inputSorters : throw new ArgumentNullException(nameof(inputSorters));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));

        foreach (var input in _inputSorters)
        {
            input.MovableUnitSelected += OnUnitSelected;
            input.FriendlyCitySelected += OnCitySelected;
            input.EnemySelected += OnCitySelected;
            input.BecomeInactive += OnDeselect;
        }
    }

    ~CitiesActionsManager()
    {
        foreach (var input in _inputSorters)
        {
            input.MovableUnitSelected -= OnUnitSelected;
            input.FriendlyCitySelected -= OnCitySelected;
            input.EnemySelected -= OnCitySelected;
            input.BecomeInactive -= OnDeselect;
        }
    }

    public event Action<Vector2Int, Side> CityCaptured;
    public event Action CitiesChanged;

    public IEnumerable<Side> Cities => _cities.Keys.Select(o => o.Side);

    public IEnumerable<Vector2Int> GetEnemyCities()
    {
        return _cities.Where(city => city.Key.Side == Side.Enemy).Select(city => _grid.GetXZ(city.Value.Position)).ToList();
    }

    public IEnumerable<Vector2Int> GetPlayerCities()
    {
        return _cities.Where(city => city.Key.Side == Side.Player).Select(city => _grid.GetXZ(city.Value.Position)).ToList();
    }

    public IEnumerable<(Vector2Int position, CitySize size, CityUnit unit)> GetEnemyCitiesUnits()
    {
        return _cities.Where(city => city.Key.Side == Side.Enemy).Select(city => (_grid.GetXZ(city.Value.Position), city.Key.CitySize, city.Key)).ToList();
    }

    public Dictionary<Vector2Int, Side> GetCities()
    {
        return _cities.ToDictionary(key => _grid.GetXZ(key.Value.Position), value => value.Key.Side);
    }

    public Dictionary<Vector2Int, CityUnit> GetInfo()
    {
        return _cities.ToDictionary(key => _grid.GetXZ(key.Value.Position), value => value.Key);
    }

    public void AddCity(CityUnit unit, ICityFacade facade, bool isVisible = false)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if (facade == null)
            throw new ArgumentNullException(nameof(facade));

        if (_cities.ContainsKey(unit))
            throw new ArgumentException("Unit already added");

        _cities.Add(unit, facade);
        CitiesChanged?.Invoke();
        _grid.SetGridObject(facade.Position, unit);

        if (unit.Side == Side.Player)
            facade.UnitView.ShowTitle();

        unit.Captured += OnCityCaptured;
        unit.Destroyed += OnUnitDied;

        if (isVisible)
            facade.UnitView.ShowTitle();

        foreach (var input in _inputSorters)
            input.Deselect();
    }

    public CityUpgrades RemoveCity(CityUnit unit)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if (_cities.ContainsKey(unit) == false)
            throw new ArgumentException("City does not exist in dictionary");

        CityUpgrades result = unit.Upgrades;
        OnUnitDied(unit);
        unit.DestroyCity();

        return result;
    }

    private void OnCityCaptured(Unit unit)
    {
        unit.Captured -= OnCityCaptured;
        Side side = unit.Side == Side.Player ? Side.Enemy : Side.Player;
        Vector2Int position = _grid.GetXZ(_cities[unit as CityUnit].Position);
        RemoveCity(unit as CityUnit);
        CityCaptured?.Invoke(position, side);
    }

    private void OnUnitSelected(Vector2Int _, IEnumerable<IEnumerable<Vector2Int>> _1,
        IEnumerable<Vector2Int> _2, IEnumerable<Vector2Int> _3, IEnumerable<Vector2Int> _4)
    {
        OnDeselect();
    }

    private void OnDeselect()
    {
        _selectedUnit?.Disable();
        _selectedUnitMenu?.Disable();
    }

    private void OnCitySelected(Vector2Int position)
    {
        _selectedUnit?.Disable();
        _selectedUnitMenu?.Disable();
        Unit unit = _grid.GetGridObject(position);

        if (unit == null || unit is CityUnit city == false)
            return;

        _selectedUnit = _cities[city].UnitView;
        _selectedUnitMenu = _cities[city].Menu;
        _selectedUnit.Enable();

        if (city.Side == GetCurrentPlayerSide())
            _selectedUnitMenu.Enable();
    }

    private void OnUnitDied(Unit unit)
    {
        unit.Destroyed -= OnUnitDied;
        var city = unit as CityUnit;
        Vector3 position = _cities[city].Position;
        _grid.SetGridObject(position, null);
        _cities.Remove(city);
        CitiesChanged?.Invoke();
    }

    private void OnDefendersSpawned(Vector2Int position)
    {
        var city = _grid.GetGridObject(position) as CityUnit;
        _cities[city].UnitView.ShowTitle();
    }

    private Side GetCurrentPlayerSide()
    {
        if (_inputSorters.Count() == 1)
            return Side.Player;

        return _inputSorters.First(o => o.IsActive).ActiveSide;
    }
}

public interface ICitiesGetter
{
    public Dictionary<Vector2Int, Side> GetCities();
}

public interface ISavedCities
{
    public Dictionary<Vector2Int, CityUnit> GetInfo();
}