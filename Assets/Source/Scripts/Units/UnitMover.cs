using System;

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
    public event Action UnitDied;

    public int Health => _health.Amount;

    public int HealthMaximum => _health.Maximum;

    public Side Side => _side;

    public int CounterAttackPower => _counterAttackPower;

    public void TakeDamage(int amount) => _health.Spent(amount);

    private void OnResourceOver() => UnitDied?.Invoke();

    private void OnHealthChanged() => HealthChanged?.Invoke();
}

public class WalkableUnit : Unit, IResetable
{
    private readonly UnitMover _mover;
    private readonly int _attackPower;

    public WalkableUnit(UnitMover mover, int attackPower,
        Side side, Resource health, int counterAttackPower) : base(side, health, counterAttackPower)
    {
        _mover = mover != null ? mover : throw new ArgumentNullException(nameof(mover));
        _attackPower = attackPower > 0 ? attackPower : throw new ArgumentOutOfRangeException(nameof(attackPower));
    }

    public void Reset()
    {
        _mover.Reset();
    }

    public int RemainingSteps => _mover.RemainingSteps;

    public int AttackPower => _attackPower;

    public bool TryMoving(int steps) => _mover.TryMoving(steps);
}

public class UnitFactory
{
    public WalkableUnit CreateInfantry(Side side)
    {
        UnitMover mover = new(2);
        Resource health = new(10);
        return new WalkableUnit(mover, 5, side, health, 4);
    }

    public Unit CreateCity(Side side)
    {
        Resource health = new(10);
        return new Unit(side, health, 4);
    }
}
