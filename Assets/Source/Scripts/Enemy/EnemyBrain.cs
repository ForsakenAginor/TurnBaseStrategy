using Assets.Scripts.HexGrid;
using HexPathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBrain : MonoBehaviour, IControllable
{
    private readonly List<WalkableUnit> _units = new();
    private readonly List<Vector2Int> _targets = new();
    private UnitsActionsManager _actionManager;
    private IUnitSpawner _unitSpawner;
    private HexGridXZ<Unit> _unitGrid;
    private HexPathFinder _pathFinder;
    private bool _isReady;
    private Coroutine _coroutine;

    public event Action<WalkableUnit, IEnumerable<Vector2Int>, Action> UnitMoving;
    public event Action<WalkableUnit, Vector3, Unit, Action> UnitAttacking;
    public event Action TurnEnded;

    public void DisableControl()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    public void EnableControl()
    {
        _coroutine = StartCoroutine(DoUnitsLogic());
    }

    private void OnDestroy()
    {
        _unitSpawner.UnitSpawned += OnUnitSpawned;
        _unitGrid.GridObjectChanged += OnGridChanged;
    }

    public void Init(HexGridXZ<Unit> grid, HexPathFinder pathfinder,
        IUnitSpawner spawner, UnitsActionsManager actionManager)
    {
        _unitGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _pathFinder = pathfinder != null ? pathfinder : throw new ArgumentNullException(nameof(pathfinder));
        _actionManager = actionManager;
        _unitSpawner = spawner;

        _unitSpawner.UnitSpawned += OnUnitSpawned;
        _unitGrid.GridObjectChanged += OnGridChanged;
    }

    private void OnGridChanged(Vector2Int position)
    {
        var unit = _unitGrid.GetGridObject(position);

        if (unit == null)
        {
            _pathFinder.MakeNodWalkable(position);
        }
        else if (unit is CityUnit && unit.Side == Side.Player)
        {
            _pathFinder.MakeNodWalkable(position);

            if (_targets.Contains(position) == false)
                _targets.Add(position);
        }
        else if (unit is CityUnit && unit.Side == Side.Enemy && _targets.Contains(position))
        {
            _targets.Remove(position);
            _pathFinder.MakeNodUnwalkable(position);
        }
        else
        {
            _pathFinder.MakeNodUnwalkable(position);
        }
    }

    private void OnUnitSpawned(Unit unit)
    {
        if (unit.Side != Side.Enemy)
            return;

        _units.Add(unit as WalkableUnit);
        unit.Destroyed += OnUnitDestroyed;
    }

    private void OnUnitDestroyed(Unit unit)
    {
        unit.Destroyed -= OnUnitDestroyed;
        _units.Remove(unit as WalkableUnit);
    }

    private IEnumerator DoUnitsLogic()
    {
        WaitUntil waitUntilAnimationPlayed = new WaitUntil(() => _isReady);
        var units = _units.ToList();

        foreach (WalkableUnit unit in units)
        {
            bool isBlocked = false;

            while (unit.IsAlive && unit.RemainingSteps > 0 && isBlocked == false)
            {
                Vector2Int position = _actionManager.GetUnitPosition(unit);

                if (CanAttack(position, out Unit targetUnit, out Vector3 targetWorldPosition))
                {
                    UnitAttacking?.Invoke(unit, targetWorldPosition, targetUnit, WaitAnimationCallback);
                    yield return waitUntilAnimationPlayed;
                    _isReady = false;
                }
                else
                {
                    if (CanMove(position, out Vector2Int target))
                    {
                        UnitMoving?.Invoke(unit, new List<Vector2Int>() { target }, WaitAnimationCallback);
                        yield return waitUntilAnimationPlayed;
                        _isReady = false;

                        if (CanAttack(target, out targetUnit, out targetWorldPosition))
                        {
                            UnitAttacking?.Invoke(unit, targetWorldPosition, targetUnit, WaitAnimationCallback);
                            yield return waitUntilAnimationPlayed;
                            _isReady = false;
                        }
                    }
                    else
                    {
                        isBlocked = true;
                    }
                }
            }
        }

        TurnEnded?.Invoke();
    }

    private void WaitAnimationCallback() => _isReady = true;

    private bool CanAttack(Vector2Int position, out Unit target, out Vector3 targetWorldPosition)
    {
        target = null;
        targetWorldPosition = Vector3.zero;
        List<Vector2Int> neighbours;

        if (_unitGrid.GetGridObject(position) is WalkableUnit unit && (unit.UnitType == UnitType.Archer || unit.UnitType == UnitType.Wizard))
            neighbours = _unitGrid.CashedFarNeighbours[position].Where(o => _unitGrid.IsValidGridPosition(o)).ToList();
        else
            neighbours = _unitGrid.CashedNeighbours[position].Where(o => _unitGrid.IsValidGridPosition(o)).ToList();

        var possibleTargets = neighbours.Where(o => IsCellContainEnemy(o)).ToList();

        if (possibleTargets.Count == 0)
            return false;

        Vector2Int targetPosition = possibleTargets.OrderBy(o => _unitGrid.GetGridObject(o).Health).First();
        targetWorldPosition = _unitGrid.GetCellWorldPosition(targetPosition);
        target = _unitGrid.GetGridObject(targetPosition);
        return true;
    }

    private bool CanMove(Vector2Int position, out Vector2Int targetPosition)
    {
        targetPosition = Vector2Int.zero;
        Vector3 coordinates = _unitGrid.GetCellWorldPosition(position);

        if (_targets.Count == 0)
            return false;

        var closestCity = _targets.OrderBy(o => Vector3.Distance(coordinates, _unitGrid.GetCellWorldPosition(o))).First();
        _pathFinder.MakeNodWalkable(position);
        var path = _pathFinder.FindPath(position, closestCity);
        _pathFinder.MakeNodUnwalkable(position);

        if (path == null)
            return false;

        targetPosition = path.Skip(1).First();
        return true;
    }

    private bool IsCellContainEnemy(Vector2Int position)
    {
        var unit = _unitGrid.GetGridObject(position);
        return unit != null && unit.Side == Side.Player;
    }
}
