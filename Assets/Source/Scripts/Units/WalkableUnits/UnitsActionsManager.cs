using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsActionsManager : IEnemyUnitOversight, ISavedUnits
{
    private readonly Dictionary<Unit, IUnitFacade> _units = new Dictionary<Unit, IUnitFacade>();
    private readonly IEnumerable<NewInputSorter> _inputSorters;
    private readonly HexGridXZ<Unit> _grid;
    private readonly IUnitActionController _enemyBrain;
    private readonly Dictionary<Side, HexGridXZ<ICloud>> _fogOfWar;
    private readonly bool _isHotSitMode;

    private ISwitchableElement _selectedUnit;

    /// <summary>
    /// Constructor fo PvE mode
    /// </summary>
    /// <param name="inputSorter"></param>
    /// <param name="grid"></param>
    /// <param name="enemyBrain"></param>
    /// <param name="cloudGrid"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UnitsActionsManager(NewInputSorter inputSorter, HexGridXZ<Unit> grid, IUnitActionController enemyBrain, Dictionary<Side, HexGridXZ<ICloud>> cloudGrid)
    {
        _inputSorters = inputSorter != null ? new List<NewInputSorter>() { inputSorter } : throw new ArgumentNullException(nameof(inputSorter));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _enemyBrain = enemyBrain != null ? enemyBrain : throw new ArgumentNullException(nameof(enemyBrain));
        _fogOfWar = cloudGrid != null ? cloudGrid : throw new ArgumentNullException(nameof(cloudGrid));

        foreach (var input in _inputSorters)
        {
            input.MovableUnitSelected += OnUnitSelected;
            input.FriendlyCitySelected += OnCitySelected;
            input.UnitIsMoving += OnUnitMoving;
            input.UnitIsAttacking += OnUnitAttacking;
            input.EnemySelected += OnEnemySelected;
            input.BecomeInactive += OnDeselect;
        }

        _enemyBrain.UnitMoving += OnUnitMoving;
        _enemyBrain.UnitAttacking += OnUnitAttacking;
    }

    /// <summary>
    /// Constructor for hotsit PvE mod
    /// </summary>
    /// <param name="inputSorters"></param>
    /// <param name="grid"></param>
    /// <param name="cloudGrid"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UnitsActionsManager(IEnumerable<NewInputSorter> inputSorters, HexGridXZ<Unit> grid, Dictionary<Side, HexGridXZ<ICloud>> cloudGrid)
    {
        _inputSorters = inputSorters != null ? inputSorters : throw new ArgumentNullException(nameof(inputSorters));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _fogOfWar = cloudGrid != null ? cloudGrid : throw new ArgumentNullException(nameof(cloudGrid));
        _isHotSitMode = true;

        foreach (var input in _inputSorters)
        {
            input.MovableUnitSelected += OnUnitSelected;
            input.FriendlyCitySelected += OnCitySelected;
            input.UnitIsMoving += OnUnitMoving;
            input.UnitIsAttacking += OnUnitAttacking;
            input.EnemySelected += OnEnemySelected;
            input.BecomeInactive += OnDeselect;
        }
    }

    ~UnitsActionsManager()
    {
        foreach (var input in _inputSorters)
        {
            input.MovableUnitSelected -= OnUnitSelected;
            input.FriendlyCitySelected -= OnCitySelected;
            input.UnitIsMoving -= OnUnitMoving;
            input.UnitIsAttacking -= OnUnitAttacking;
            input.EnemySelected -= OnEnemySelected;
            input.BecomeInactive -= OnDeselect;
        }

        if (_enemyBrain != null)
        {
            _enemyBrain.UnitMoving -= OnUnitMoving;
            _enemyBrain.UnitAttacking -= OnUnitAttacking;
        }
    }

    public event Action<Vector2Int> EnemyDoSomething;

    public IEnumerable<IResetable> Units => _units.Keys.Where(o => o is WalkableUnit).Select(o => o as IResetable);

    public Dictionary<Vector2Int, WalkableUnit> GetInfo()
    {
        return _units.ToDictionary(key => _grid.GetXZ(key.Value.Position), value => value.Key as WalkableUnit);
    }

    public void AddUnit(Unit unit, IUnitFacade facade)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if (facade == null)
            throw new ArgumentNullException(nameof(facade));

        if (_units.ContainsKey(unit))
            throw new ArgumentException("Unit already added");

        _units.Add(unit, facade);
        _grid.SetGridObject(facade.Position, unit);

        unit.Destroyed += OnUnitDied;

        foreach (var input in _inputSorters)
            input.Deselect();
    }

    public Vector2Int GetUnitPosition(Unit unit) => _grid.GetXZ(_units[unit].Position);

    private void OnDeselect() => _selectedUnit?.Disable();

    private void OnCitySelected(Vector2Int position) => OnDeselect();

    private void OnEnemySelected(Vector2Int position) => OnUnitSelected(position, null, null, null, null);

    private void OnUnitSelected(Vector2Int unitPosition,
        IEnumerable<IEnumerable<Vector2Int>> _, IEnumerable<Vector2Int> _1,
        IEnumerable<Vector2Int> _2, IEnumerable<Vector2Int> _3)
    {
        _selectedUnit?.Disable();
        Unit unit = _grid.GetGridObject(unitPosition);

        if (unit == null || _units.ContainsKey(unit) == false)
            return;

        _selectedUnit = _units[unit].UnitView;
        _selectedUnit.Enable();
    }

    private void OnUnitDied(Unit unit)
    {
        unit.Destroyed -= OnUnitDied;
        Vector3 position = _units[unit].Position;
        _grid.SetGridObject(position, null);

        if (_selectedUnit == _units[unit].UnitView)
            _selectedUnit = null;

        _units.Remove(unit);
    }

    private void OnUnitAttacking(WalkableUnit unit1, Vector3 targetPosition, Unit unit2, Action callback)
    {
        Side enemy = GetCurrentEnemySide();
        IUnitFacade unitFacade = _units[unit1];

        if (unit1.CanAttack)
        {
            if (unitFacade is IWalkableUnitFacade facade)
            {
                facade.Attacker.Attack(targetPosition, callback, () => unit1.TryAttack(unit2));

                if ((unit1.Side == enemy && unit2.Side != Side.Neutral)
                    || (_isHotSitMode && unit2.Side != Side.Neutral))
                    EnemyDoSomething?.Invoke(_grid.GetXZ(targetPosition));
            }
            else
            {
                throw new Exception();
            }
        }
        else
        {
            callback.Invoke();
            return;
        }
    }

    private void OnUnitMoving(WalkableUnit unit, IEnumerable<Vector2Int> target, Action callback)
    {
        int steps = 0;

        if (target != null)
            steps = target.Count();
        else
            callback.Invoke();

        if (unit.TryMoving(steps))
        {
            if (_units[unit] is IWalkableUnitFacade facade)
            {
                var cloud = _fogOfWar[GetCurrentPlayerSide()].GetGridObject(target.Last());
                List<Vector3> way = new();

                foreach (var step in target)
                    way.Add(_grid.GetCellWorldPosition(step));

                if (cloud == null || cloud.IsDissappeared == true)
                {
                    facade.Mover.Move(way, callback);

                    if (_isHotSitMode || unit.Side == GetCurrentEnemySide())
                        EnemyDoSomething?.Invoke(target.Last());
                }
                else if (_isHotSitMode)
                {
                    facade.Mover.Move(way, callback);
                }
                else
                {
                    facade.Mover.MoveFast(way, callback);
                }
            }
            else
            {
                throw new Exception("You are moving unmovable object");
            }

            Vector3 position = _units[unit].Position;
            _grid.SetGridObject(position, null);
            _grid.SetGridObject(target.Last().x, target.Last().y, unit);
        }
        else
        {
            callback.Invoke();
        }
    }

    private Side GetCurrentEnemySide()
    {
        if (_inputSorters.Count() == 1)
            return _inputSorters.First().Enemy;

        return _inputSorters.First(o => o.IsActive).Enemy;
    }

    private Side GetCurrentPlayerSide()
    {
        if (_inputSorters.Count() == 1)
            return _inputSorters.First().ActiveSide;

        return _inputSorters.First(o => o.IsActive).ActiveSide;
    }
}

public interface IEnemyUnitOversight
{
    public event Action<Vector2Int> EnemyDoSomething;
}

public interface ISavedUnits
{
    public Dictionary<Vector2Int, WalkableUnit> GetInfo();
}