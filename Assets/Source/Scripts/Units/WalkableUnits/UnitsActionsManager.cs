using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsActionsManager : IEnemyUnitOversight, ISavedUnits
{
    private readonly Dictionary<Unit, IUnitFacade> _units = new Dictionary<Unit, IUnitFacade>();
    private readonly NewInputSorter _inputSorter;
    private readonly HexGridXZ<Unit> _grid;
    private readonly EnemyBrain _enemyBrain;
    private readonly HexGridXZ<ICloud> _fogOfWar;

    private ISwitchableElement _selectedUnit;

    public UnitsActionsManager(NewInputSorter inputSorter, HexGridXZ<Unit> grid, EnemyBrain enemyBrain, HexGridXZ<ICloud> cloudGrid)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new ArgumentNullException(nameof(inputSorter));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _enemyBrain = enemyBrain != null ? enemyBrain : throw new ArgumentNullException(nameof(enemyBrain));
        _fogOfWar = cloudGrid != null ? cloudGrid : throw new ArgumentNullException(nameof(cloudGrid));

        _inputSorter.MovableUnitSelected += OnUnitSelected;
        _inputSorter.FriendlyCitySelected += OnCitySelected;
        _inputSorter.UnitIsMoving += OnUnitMoving;
        _inputSorter.UnitIsAttacking += OnUnitAttacking;
        _inputSorter.EnemySelected += OnEnemySelected;
        _inputSorter.BecomeInactive += OnDeselect;

        _enemyBrain.UnitMoving += OnUnitMoving;
        _enemyBrain.UnitAttacking += OnUnitAttacking;
    }

    ~UnitsActionsManager()
    {
        _inputSorter.MovableUnitSelected -= OnUnitSelected;
        _inputSorter.FriendlyCitySelected -= OnCitySelected;
        _inputSorter.UnitIsMoving -= OnUnitMoving;
        _inputSorter.UnitIsAttacking -= OnUnitAttacking;
        _inputSorter.EnemySelected -= OnEnemySelected;
        _inputSorter.BecomeInactive -= OnDeselect;

        _enemyBrain.UnitMoving -= OnUnitMoving;
        _enemyBrain.UnitAttacking -= OnUnitAttacking;
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

        _inputSorter.Deselect();
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
        IUnitFacade unitFacade = _units[unit1];

        if (unit1.CanAttack)
        {
            if (unitFacade is IWalkableUnitFacade facade)
            {
                facade.Attacker.Attack(targetPosition, callback, () => unit1.TryAttack(unit2));

                if (unit1.Side == Side.Enemy)
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
                var cloud = _fogOfWar.GetGridObject(target.Last());
                List<Vector3> way = new();

                foreach (var step in target)
                    way.Add(_grid.GetCellWorldPosition(step));

                if (cloud == null || cloud.IsDissappeared == true)
                {
                    facade.Mover.Move(way, callback);
                    EnemyDoSomething?.Invoke(target.Last());
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
}

public interface IEnemyUnitOversight
{
    public event Action<Vector2Int> EnemyDoSomething;
}

public interface ISavedUnits
{
    public Dictionary<Vector2Int, WalkableUnit> GetInfo();
}