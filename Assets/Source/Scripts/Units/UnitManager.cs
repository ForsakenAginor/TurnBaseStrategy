using Assets.Scripts.HexGrid;
using HexPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager
{
    private readonly Dictionary<Unit, Transform> _units = new Dictionary<Unit, Transform>();
    private readonly InputSorter _inputSorter;
    private readonly HexGridXZ<Unit> _grid;
    private readonly HexPathFinder _pathfinder;

    public UnitManager(InputSorter inputSorter, HexGridXZ<Unit> grid, HexPathFinder pathfinder)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new System.ArgumentNullException(nameof(inputSorter));
        _grid = grid != null ? grid : throw new System.ArgumentNullException(nameof(grid));
        _pathfinder = pathfinder != null ? pathfinder : throw new ArgumentNullException(nameof(pathfinder));

        _inputSorter.RoutSubmited += OnRoutSubmited;
    }

    ~UnitManager()
    {
        _inputSorter.RoutSubmited -= OnRoutSubmited;
    }

    public IEnumerable<IResetable> Units => _units.Keys.Where(o => o is WalkableUnit).Select(o => o as IResetable);

    public void AddUnit(Unit unit, Transform transform)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if (transform == null)
            throw new ArgumentNullException(nameof(transform));

        if (_units.ContainsKey(unit))
            throw new ArgumentException("Unit already added");

        _units.Add(unit, transform);
        _grid.SetGridObject(transform.transform.position, unit);
        Vector2Int coordinates = _grid.GetXZ(transform.transform.position);

        if (unit.Side == Side.Enemy)
            _pathfinder.MakeNodUnwalkable(coordinates);
    }

    private void OnRoutSubmited(Rout rout)
    {
        if (_grid.GetGridObject(rout.SelectedCell) is WalkableUnit unit)
        {
            if (rout.ClosePartOfPath.Count > 0 && unit.TryMoving(rout.ClosePartOfPath.Count))
            {
                _units[unit].GetComponent<Mover>().Move(rout.Path);
                _grid.SetGridObject(rout.SelectedCell.x, rout.SelectedCell.y, null);
                _grid.SetGridObject(rout.ClosePartOfPath.Last().x, rout.ClosePartOfPath.Last().y, unit);
            }
        }
    }
}