using Assets.Scripts.HexGrid;
using HexPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsManager
{
    private readonly Dictionary<Unit, IUnitFacade> _units = new Dictionary<Unit, IUnitFacade>();
    private readonly NewInputSorter _inputSorter;
    private readonly HexGridXZ<Unit> _grid;
    private readonly HexPathFinder _pathfinder;

    public UnitsManager(NewInputSorter inputSorter, HexGridXZ<Unit> grid, HexPathFinder pathfinder)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new System.ArgumentNullException(nameof(inputSorter));
        _grid = grid != null ? grid : throw new System.ArgumentNullException(nameof(grid));
        _pathfinder = pathfinder != null ? pathfinder : throw new ArgumentNullException(nameof(pathfinder));

        _inputSorter.UnitIsMoving += OnUnitMoving;
        _inputSorter.UnitIsAttacking += OnUnitAttacking;
    }

    ~UnitsManager()
    {
        _inputSorter.UnitIsMoving -= OnUnitMoving;
        _inputSorter.UnitIsAttacking -= OnUnitAttacking;
    }

    public IEnumerable<IResetable> Units => _units.Keys.Where(o => o is WalkableUnit).Select(o => o as IResetable);

    public void AddUnit(Unit unit, IUnitFacade facade)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if (facade == null)
            throw new ArgumentNullException(nameof(facade));

        if (_units.ContainsKey(unit))
            throw new ArgumentException("Unit already added");

        _units.Add(unit, facade);
        facade.UnitView.Init(unit);
        _grid.SetGridObject(facade.Position, unit);
        Vector2Int coordinates = _grid.GetXZ(facade.Position);

        if (unit.Side == Side.Enemy)
            _pathfinder.MakeNodUnwalkable(coordinates);

        unit.Died += OnUnitDied;
    }

    private void OnUnitDied(Unit unit)
    {
        Vector3 position = _units[unit].Position;
        _grid.SetGridObject(position, null);
        _units.Remove(unit);
    }

    private void OnUnitAttacking(WalkableUnit unit1, Unit unit2, Action callback)
    {
        if (unit1.TryAttack(unit2) == false)
        {
            callback.Invoke();
            return;
        }

        //todo: wait animation end
        callback.Invoke();
    }

    private void OnUnitMoving(WalkableUnit unit, Vector2Int target, Action callback)
    {
        int step = 1;

        if (unit.TryMoving(step))
        {
            if (_units[unit] is IWalkableUnitFacade facade)
                facade.Mover.Move(_grid.GetCellWorldPosition(target), callback);
            else
                throw new Exception("You are moving unmovable object");
            
            Vector3 position = _units[unit].Position;
            _grid.SetGridObject(position, null);
            _grid.SetGridObject(target.x, target.y, unit);
        }
        else
        {
            callback.Invoke();
        }
    }
}