using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CitiesActionsManager
{
    private readonly Dictionary<CityUnit, ICityFacade> _cities = new Dictionary<CityUnit, ICityFacade>();
    private readonly NewInputSorter _inputSorter;
    private readonly HexGridXZ<Unit> _grid;

    private IUIElement _selectedUnit;
    private IUIElement _selectedUnitMenu;

    public CitiesActionsManager(NewInputSorter inputSorter, HexGridXZ<Unit> grid)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new System.ArgumentNullException(nameof(inputSorter));
        _grid = grid != null ? grid : throw new System.ArgumentNullException(nameof(grid));

        _inputSorter.MovableUnitSelected += OnUnitSelected;
        _inputSorter.FriendlyCitySelected += OnCitySelected;
        _inputSorter.EnemySelected += OnCitySelected;
        _inputSorter.BecomeInactive += OnDeselect;
    }

    ~CitiesActionsManager()
    {
        _inputSorter.MovableUnitSelected += OnUnitSelected;
        _inputSorter.FriendlyCitySelected -= OnCitySelected;
        _inputSorter.EnemySelected -= OnCitySelected;
        _inputSorter.BecomeInactive -= OnDeselect;
    }

    public event Action<Vector2Int, Side> CityCaptured;

    public void AddCity(CityUnit unit, ICityFacade facade)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if (facade == null)
            throw new ArgumentNullException(nameof(facade));

        if (_cities.ContainsKey(unit))
            throw new ArgumentException("Unit already added");

        _cities.Add(unit, facade);
        _grid.SetGridObject(facade.Position, unit);

        unit.Captured += OnCityCaptured;
        unit.Destroyed += OnUnitDied;
    }

    public void RemoveCity(CityUnit unit)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if (_cities.ContainsKey(unit) == false)
            throw new ArgumentException("City does not exist in dictionary");

        OnUnitDied(unit);
        unit.DestroyCity();
    }

    private void OnCityCaptured(Unit unit)
    {
        unit.Captured -= OnCityCaptured;
        Side side = unit.Side == Side.Player ? Side.Enemy : Side.Player;
        Vector2Int position = _grid.GetXZ(_cities[unit as CityUnit].Position);
        RemoveCity(unit as CityUnit);
        CityCaptured?.Invoke(position, side);
    }

    private void OnUnitSelected(Vector2Int _, IEnumerable<Vector2Int> _1,
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
        _selectedUnitMenu.Enable();
    }

    private void OnUnitDied(Unit unit)
    {
        unit.Destroyed -= OnUnitDied;
        var city = unit as CityUnit;
        Vector3 position = _cities[city].Position;
        _grid.SetGridObject(position, null);
        _cities.Remove(city);
    }
}