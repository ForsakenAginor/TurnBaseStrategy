using System;
using UnityEngine;

public class UnitMover : IResetable
{
    private readonly int _maxSteps;
    private int _remainingSteps;

    public UnitMover(int maxSteps)
    {
        _maxSteps = maxSteps > 0 ? maxSteps : throw new System.ArgumentOutOfRangeException(nameof(maxSteps));
        _remainingSteps = maxSteps;
    }

    public void Reset()
    {
        _remainingSteps = _maxSteps;
    }

    public int RemainingSteps => _remainingSteps;

    public bool TryMoving(int steps)
    {
        if (steps <= 0)
            throw new ArgumentOutOfRangeException(nameof(steps));

        if (_remainingSteps < steps)
            return false;

        _remainingSteps -= steps;
        return true;
    }

    public void SpentAllSteps()
    {
        _remainingSteps = 0;
    }
}

public enum Side
{
    Player,
    Enemy,
}

public class Unit
{
    private readonly Side _side;
    private readonly Resource _health;
    private readonly int _counterAttackPower;

    public Unit(Side side, Resource health, int counterAttackPower)
    {
        _side = side;
        _health = health != null ? health : throw new ArgumentNullException(nameof(health));
        _counterAttackPower = counterAttackPower >= 0 ? counterAttackPower : throw new ArgumentOutOfRangeException(nameof(counterAttackPower));

        _health.ResourcesAmountChanged += OnHealthChanged;
        _health.ResourceOver += OnResourceOver;
    }

    ~Unit()
    {
        _health.ResourcesAmountChanged -= OnHealthChanged;
        _health.ResourceOver -= OnResourceOver;
    }

    public event Action HealthChanged;
    public event Action<Unit> Died;

    public int Health => _health.Amount;

    public int HealthMaximum => _health.Maximum;

    public Side Side => _side;

    public int CounterAttackPower => _counterAttackPower;

    public void TakeDamage(int amount) => _health.Spent(amount);

    private void OnResourceOver() => Died?.Invoke(this);

    private void OnHealthChanged() => HealthChanged?.Invoke();
}

public class WalkableUnit : Unit, IResetable
{
    private readonly UnitMover _mover;
    private readonly int _attackPower;
    private bool _canAttack = true;

    public WalkableUnit(UnitMover mover, int attackPower,
        Side side, Resource health, int counterAttackPower) : base(side, health, counterAttackPower)
    {
        _mover = mover != null ? mover : throw new ArgumentNullException(nameof(mover));
        _attackPower = attackPower > 0 ? attackPower : throw new ArgumentOutOfRangeException(nameof(attackPower));
    }

    public event Action Moved;

    public void Reset()
    {
        _canAttack = true;
        _mover.Reset();
        Moved?.Invoke();
    }

    public bool CanAttack => _canAttack;

    public int RemainingSteps => _mover.RemainingSteps;

    public int AttackPower => _attackPower;

    public bool TryMoving(int steps)
    {
        if(_mover.TryMoving(steps) == false)
            return false;

        Moved?.Invoke();
        return true;
    }

    public bool TryAttack(Unit target)
    {
        if (_canAttack == false)
            return false;

        _canAttack = false;
        _mover.SpentAllSteps();
        Moved?.Invoke();
        target.TakeDamage(_attackPower);
        TakeDamage(target.CounterAttackPower);
        return true;
    }
}

public interface IUnitFacade
{
    public Vector3 Position { get;}

    public UnitView UnitView { get; }
}

public interface IWalkableUnitFacade : IUnitFacade
{
    public Mover Mover { get; }
}

public class UnitFactory
{
    private readonly IUnitInfoGetter _configuration;

    public UnitFactory(IUnitInfoGetter configuration)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
    }

    public WalkableUnit CreateInfantry(Side side)
    {
        UnitType infantry = UnitType.Infantry;
        var tuple = _configuration.GetUnitInfo(infantry);
        UnitMover mover = new(tuple.steps);
        Resource health = new(tuple.health);
        return new WalkableUnit(mover, tuple.attack, side, health, tuple.counterAttack);
    }

    public Unit CreateCity(Side side)
    {
        Resource health = new(10);
        return new Unit(side, health, 4);
    }
}
