using System;

public class WalkableUnit : Unit, IResetable
{
    private readonly UnitMover _mover;
    private readonly int _attackPower;
    private readonly UnitType _type;
    private bool _canAttack = true;

    public WalkableUnit(UnitMover mover, int attackPower, UnitType type,
        Side side, Resource health, int counterAttackPower) : base(side, health, counterAttackPower)
    {
        _mover = mover != null ? mover : throw new ArgumentNullException(nameof(mover));
        _attackPower = attackPower > 0 ? attackPower : throw new ArgumentOutOfRangeException(nameof(attackPower));
        _type = type;
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

    public UnitType UnitType => _type;

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
