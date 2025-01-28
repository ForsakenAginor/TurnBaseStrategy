using Assets.Scripts.HexGrid;
using HexPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewInputSorter : IControllable, IWaitAnimation
{
    private readonly HexGridXZ<Unit> _hexGrid;
    private readonly HexGridXZ<IHexOnScene> _blockedCells;
    private readonly CellSelector _cellSelector;
    private readonly Vector2Int _fakeCell = new Vector2Int(-10000, -10000);
    private readonly HexPathFinder _pathFinder;

    private Vector2Int _selectedCell;
    private List<List<Vector2Int>> _possibleWays;
    private List<Vector2Int> _possibleAttacks;
    private WalkableUnit _selectedUnit;
    private bool _isActive;

    public NewInputSorter(HexGridXZ<Unit> grid, CellSelector selector, HexGridXZ<IHexOnScene> blockedCells, HexPathFinder pathFinder)
    {
        _hexGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _blockedCells = blockedCells != null ? blockedCells : throw new ArgumentNullException(nameof(blockedCells));
        _cellSelector = selector != null ? selector : throw new ArgumentNullException(nameof(selector));
        _pathFinder = pathFinder != null ? pathFinder : throw new ArgumentNullException(nameof(pathFinder));

        _hexGrid.GridObjectChanged += OnGridObjectChanged;
    }

    ~NewInputSorter()
    {
        _hexGrid.GridObjectChanged -= OnGridObjectChanged;
    }

    public event Action<Vector2Int, IEnumerable<IEnumerable<Vector2Int>>, IEnumerable<Vector2Int>, IEnumerable<Vector2Int>, IEnumerable<Vector2Int>> MovableUnitSelected;
    public event Action<WalkableUnit, Vector3, Unit, Action> UnitIsAttacking;
    public event Action<WalkableUnit, IEnumerable<Vector2Int>, Action> UnitIsMoving;
    public event Action<Vector2Int> FriendlyCitySelected;
    public event Action<Vector2Int> EnemySelected;
    public event Action BecomeInactive;
    public event Action AnimationComplete;

    public bool IsAnimationPlayed { get; private set; } = true;


    public void EnableControl()
    {
        _selectedCell = _fakeCell;
        _selectedUnit = null;
        _possibleWays = null;
        _possibleAttacks = null;
        _cellSelector.CellClicked += OnCellClicked;
        _isActive = true;
    }

    public void DisableControl()
    {
        BecomeInactive?.Invoke();
        _cellSelector.CellClicked -= OnCellClicked;
        _isActive = false;
    }

    private void OnGridObjectChanged(Vector2Int position)
    {
        var unit = _hexGrid.GetGridObject(position);

        if (unit == null)
            _pathFinder.MakeNodWalkable(position);
        else if (unit.Side == Side.Enemy)
            _pathFinder.MakeNodUnwalkable(position);
        else
            _pathFinder.MakeNodWalkable(position);
    }

    private void OnCellClicked(Vector3 worldPosition, Vector2Int position)
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
            var farNeighbours = _hexGrid.CashedFarNeighbours[position].Where(o => _hexGrid.IsValidGridPosition(o)).ToList();

            FindPossibleWays(neighbours, farNeighbours, position);
            var blockedCells = neighbours.Where(o => IsCellUnwalkable(o)).ToList();
            var friendlyCells = neighbours.Where(o => IsCellContainAlly(_selectedUnit, o)).ToList();

            if (selectedUnit.UnitType == UnitType.Archer)
                neighbours = _hexGrid.CashedFarNeighbours[position].Where(o => _hexGrid.IsValidGridPosition(o)).ToList();

            _possibleAttacks = neighbours.Where(o => IsCellContainEnemy(_selectedUnit, o)).ToList();
            MovableUnitSelected?.Invoke(position, _possibleWays, blockedCells, friendlyCells, _possibleAttacks);
            return;
        }
        // Friendly City chosen
        else if (unit != null && unit.Side == Side.Player && unit is WalkableUnit == false)
        {
            _selectedCell = position;
            _possibleWays = null;
            _possibleAttacks = null;
            FriendlyCitySelected?.Invoke(position);
            return;
        }

        // if selected unit moving to free cell
        if (_possibleWays != null && IsCellInPossibleWays(position, out IEnumerable<Vector2Int> path))
        {
            _possibleWays = null;
            _possibleAttacks = null;
            _cellSelector.CellClicked -= OnCellClicked;
            IsAnimationPlayed = false;

            UnitIsMoving?.Invoke(_selectedUnit, path, () =>
                {
                    if (_isActive)
                        _cellSelector.CellClicked += OnCellClicked;

                    IsAnimationPlayed = true;
                    AnimationComplete?.Invoke();
                    OnCellClicked(worldPosition, position);
                });
            return;
        }

        // if selected unit attack
        if (_possibleWays != null && _possibleAttacks.Contains(position))
        {
            _possibleWays = null;
            _possibleAttacks = null;
            _cellSelector.CellClicked -= OnCellClicked;
            IsAnimationPlayed = false;

            UnitIsAttacking?.Invoke(_selectedUnit, worldPosition, unit, () =>
                {
                    if (_isActive)
                        _cellSelector.CellClicked += OnCellClicked;

                    IsAnimationPlayed = true;
                    AnimationComplete?.Invoke();
                    OnCellClicked(worldPosition, position);
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

    private bool IsCellFree(Vector2Int position)
    {
        return _hexGrid.GetGridObject(position) == null
            && _blockedCells.GetGridObject(position).IsBlocked == false;
    }

    private bool IsCellInPossibleWays(Vector2Int position, out IEnumerable<Vector2Int> path)
    {
        path = null;
        bool result = false;

        foreach (var way in _possibleWays)
        {
            if (way.Last() == position)
            {
                path = way;
                result = true;
            }
        }

        return result;
    }

    private void FindPossibleWays(List<Vector2Int> neighbours, List<Vector2Int> farNeighbours, Vector2Int unitPosition)
    {
        if (_selectedUnit.RemainingSteps > 1)
        {
            _possibleWays = new List<List<Vector2Int>>();

            foreach (var cell in farNeighbours)
            {
                if (IsCellFree(cell) == false)
                    continue;

                var path = _pathFinder.FindPath(unitPosition, cell);
                int maxPathLength = 3;

                if (path != null && path.Count <= maxPathLength)
                    _possibleWays.Add(path.Skip(1).ToList());
            }
        }
        else if (_selectedUnit.RemainingSteps == 1)
        {
            _possibleWays = new List<List<Vector2Int>>();

            foreach (var cell in neighbours)
                if (IsCellFree(cell))
                    _possibleWays.Add(new List<Vector2Int>() { cell });
        }
        else
        {
            _possibleWays = new();
        }
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

public interface IWaitAnimation
{
    public bool IsAnimationPlayed { get; }

    public event Action AnimationComplete;
}