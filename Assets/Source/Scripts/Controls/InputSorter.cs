using Assets.Scripts.HexGrid;
using HexPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputSorter : IControllable
{
    private readonly HexGridXZ<Unit> _hexGrid;
    private readonly CellSelector _cellSelector;
    private readonly HexPathFinder _pathFinder;
    private readonly Vector2Int _fakeCell = new Vector2Int(-10000, -10000);

    private Vector2Int _selectedCell;
    private Vector2Int _lastClickedCell;
    private Rout _rout;
    private Unit _selectedUnit;

    public InputSorter(HexGridXZ<Unit> grid, CellSelector selector, HexPathFinder pathFinder)
    {
        _hexGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _cellSelector = selector != null ? selector : throw new ArgumentNullException(nameof(selector));
        _pathFinder = pathFinder != null ? pathFinder : throw new ArgumentNullException(nameof(pathFinder));
    }

    public event Action<Rout> PathCreated;
    public event Action<Vector2Int, Side> SelectionChanged;
    public event Action<Rout> RoutSubmited;
    public event Action BecomeInactive;

    public void EnableControl()
    {
        _selectedCell = _fakeCell;
        _lastClickedCell = _fakeCell;
        _cellSelector.CellClicked += OnCellClicked;
    }

    public void DisableControl()
    {
        BecomeInactive?.Invoke();
        _cellSelector.CellClicked -= OnCellClicked;
    }

    private void OnCellClicked(Vector3 _, Vector2Int position)
    {
        if (position == _selectedCell)
            return;

        var unit = _hexGrid.GetGridObject(position);

        if (unit != null)
        {
            _selectedCell = position;
            _selectedUnit = unit;
            SelectionChanged?.Invoke(_selectedCell, _selectedUnit.Side);
            return;
        }

        if (_selectedUnit.Side == Side.Enemy)
            return;

        if (_lastClickedCell != position)
        {
            CreatePath(position);
            _lastClickedCell = position;
            return;
        }

        var closePath = _rout.ClosePartOfPath;

        if (closePath.Count == 0)
            return;

        var endPosition = closePath.Last();
        unit = _hexGrid.GetGridObject(endPosition);

        if (unit != null)
        {
            _lastClickedCell = _fakeCell;
            SelectionChanged?.Invoke(_selectedCell, unit.Side);
            return;
        }

        RoutSubmited?.Invoke(_rout);
        _selectedCell = _fakeCell;
        _lastClickedCell = _fakeCell;
    }

    private void CreatePath(Vector2Int position)
    {
        var path = _pathFinder.FindPath(_selectedCell, position);

        if (path == null)
            return;

        path = path.Skip(1).ToList();
        List<Vector2Int> farawayPartOfPath = new List<Vector2Int>();
        WalkableUnit activeUnit = _hexGrid.GetGridObject(_selectedCell) as WalkableUnit;

        if (activeUnit.RemainingSteps < path.Count)
        {
            farawayPartOfPath = path.Skip(activeUnit.RemainingSteps).ToList();
            path = path.Take(activeUnit.RemainingSteps).ToList();
        }

        List<Vector3> worldCoordinatesPath = new();
        worldCoordinatesPath = path.Select(o => _hexGrid.GetCellWorldPosition(o)).ToList();

        _rout = new Rout(path, farawayPartOfPath, _selectedCell, worldCoordinatesPath);
        PathCreated?.Invoke(_rout);
    }
}
