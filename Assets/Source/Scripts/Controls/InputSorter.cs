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
        /*
        _selectedCell = _fakeCell;
        _lastClickedCell = _fakeCell;
        _cellSelector.CellClicked += OnCellClicked;*/
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

public class TestInputSorter : IControllable
{
    private readonly HexGridXZ<Unit> _hexGrid;
    private readonly HexGridXZ<IBlockedCell> _blockedCells;
    private readonly CellSelector _cellSelector;
    private readonly Vector2Int _fakeCell = new Vector2Int(-10000, -10000);

    private Vector2Int _selectedCell;
    private List<Vector2Int> _possibleWays;
    private List<Vector2Int> _possibleAttacks;
    private WalkableUnit _selectedUnit;

    public TestInputSorter(HexGridXZ<Unit> grid, CellSelector selector, HexGridXZ<IBlockedCell> blockedCells)
    {
        _hexGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _blockedCells = blockedCells != null ? blockedCells : throw new ArgumentNullException(nameof(blockedCells));
        _cellSelector = selector != null ? selector : throw new ArgumentNullException(nameof(selector));
    }

    public event Action<IEnumerable<Vector2Int>, IEnumerable<Vector2Int>, IEnumerable<Vector2Int>, IEnumerable<Vector2Int>> MovableUnitSelected;
    public event Action<WalkableUnit, Unit, Action> UnitIsAttacking;
    public event Action<WalkableUnit, Vector2Int, Action> UnitIsMoving;

    public event Action<Vector2Int> EnemySelected;
    public event Action BecomeInactive;

    public void EnableControl()
    {
        _selectedCell = _fakeCell;
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

        // Walkable friendly unit chosen
        if (unit != null && unit.Side == Side.Player && unit is WalkableUnit selectedUnit)
        {
            _selectedCell = position;
            _selectedUnit = selectedUnit;
            var neighbours = _hexGrid.CashedNeighbours[position].Where(o => _hexGrid.IsValidGridPosition(o)).ToList();
            _possibleWays = neighbours.Where(o => IsCellFree(_selectedUnit, o)).ToList();
            var blockedCells = neighbours.Where(o => IsCellUnwalkable(o)).ToList();
            var friendlyCells = neighbours.Where(o => IsCellContainAlly(_selectedUnit, o)).ToList();
            _possibleAttacks = neighbours.Where(o => IsCellContainEnemy(_selectedUnit, o)).ToList();

            MovableUnitSelected?.Invoke(_possibleWays, blockedCells, friendlyCells, _possibleAttacks);
            return;
        }

        // if selected unit moving to free cell
        if (_possibleWays != null && _possibleWays.Contains(position))
        {
            _possibleWays = null;
            _possibleAttacks = null;
            _cellSelector.CellClicked -= OnCellClicked;

            UnitIsMoving?.Invoke(_selectedUnit, position, () =>
                {
                    _cellSelector.CellClicked += OnCellClicked;
                    OnCellClicked(_, position);
                });
            return;
        }

        // if selected unit attack
        if (_possibleWays != null && _possibleAttacks.Contains(position))
        {
            _possibleWays = null;
            _possibleAttacks = null;
            _cellSelector.CellClicked -= OnCellClicked;

            UnitIsAttacking?.Invoke(_selectedUnit, unit, () =>
                {
                    _cellSelector.CellClicked += OnCellClicked;
                    OnCellClicked(_, position);
                });
            return;
        }

        // if enemy selected
        if (unit != null && unit.Side == Side.Enemy)
        {
            _selectedCell = position;
            _possibleWays = null;
            _possibleAttacks = null;

            EnemySelected?.Invoke(position);
            return;
        }

        // deselect
        _selectedCell = position;
        _possibleWays = null;
        _possibleAttacks = null;
        BecomeInactive?.Invoke();
    }

    private bool IsCellFree(WalkableUnit unit, Vector2Int position)
    {
        return unit.RemainingSteps > 0
            && _hexGrid.GetGridObject(position) == null
            && _blockedCells.GetGridObject(position).IsBlocked == false;
    }

    private bool IsCellUnwalkable(Vector2Int position)
    {
        return _blockedCells.GetGridObject(position).IsBlocked == true;
    }

    private bool IsCellContainEnemy(WalkableUnit unit, Vector2Int position)
    {
        var cell = _hexGrid.GetGridObject(position);
        return unit.CanAttack && cell != null && cell.Side == Side.Enemy;
    }

    private bool IsCellContainAlly(WalkableUnit unit, Vector2Int position)
    {
        var cell = _hexGrid.GetGridObject(position);
        return unit.CanAttack && cell != null && cell.Side == Side.Player;
    }
}