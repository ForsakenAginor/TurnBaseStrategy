using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewInputSorter : IControllable
{
    private readonly HexGridXZ<Unit> _hexGrid;
    private readonly HexGridXZ<IBlockedCell> _blockedCells;
    private readonly CellSelector _cellSelector;
    private readonly Vector2Int _fakeCell = new Vector2Int(-10000, -10000);

    private Vector2Int _selectedCell;
    private List<Vector2Int> _possibleWays;
    private List<Vector2Int> _possibleAttacks;
    private WalkableUnit _selectedUnit;

    public NewInputSorter(HexGridXZ<Unit> grid, CellSelector selector, HexGridXZ<IBlockedCell> blockedCells)
    {
        _hexGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _blockedCells = blockedCells != null ? blockedCells : throw new ArgumentNullException(nameof(blockedCells));
        _cellSelector = selector != null ? selector : throw new ArgumentNullException(nameof(selector));
    }

    public event Action<Vector2Int, IEnumerable<Vector2Int>, IEnumerable<Vector2Int>, IEnumerable<Vector2Int>, IEnumerable<Vector2Int>> MovableUnitSelected;
    public event Action<WalkableUnit, Unit, Action> UnitIsAttacking;
    public event Action<WalkableUnit, Vector2Int, Action> UnitIsMoving;
    public event Action<Vector2Int> FriendlyCitySelected;
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