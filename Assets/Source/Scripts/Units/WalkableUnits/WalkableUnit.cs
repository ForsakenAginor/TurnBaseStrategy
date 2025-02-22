﻿using System;

public class WalkableUnit : Unit, IResetable
{
    private readonly UnitMover _mover;
    private readonly int _attackPower;
    private readonly UnitType _type;
    private bool _canAttack = true;

    public WalkableUnit(UnitMover mover, int attackPower, UnitType type,
        Side side, Resource health, int counterAttackPower, bool canAttackStatus = true) : base(side, health, counterAttackPower)
    {
        _mover = mover != null ? mover : throw new ArgumentNullException(nameof(mover));
        _attackPower = attackPower > 0 ? attackPower : throw new ArgumentOutOfRangeException(nameof(attackPower));
        _type = type;
        _canAttack = canAttackStatus;
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
        if (_mover.TryMoving(steps) == false)
            return false;

        Moved?.Invoke();
        return true;
    }

    public bool TryAttack(Unit target)
    {
        if (_canAttack == false)
            return false;

        _canAttack = false;

        //******** Spearman attack bonus vs Cavalry ************
        int dealingDamageFactor = _type == UnitType.Spearman
            && target is WalkableUnit knight
            && knight.UnitType == UnitType.Knight ?
            1 : 0;
        int takingDamageFactor = _type == UnitType.Knight
            && target is WalkableUnit spearman
            && spearman.UnitType == UnitType.Spearman ?
            1 : 0;

        _mover.SpentAllSteps();
        Moved?.Invoke();
        target.TakeDamage(_attackPower + dealingDamageFactor);

        // Archer and Wizard ignore counter attack logic
        if (_type == UnitType.Archer || _type == UnitType.Wizard)
            return true;

        TakeDamage(target.CounterAttackPower + takingDamageFactor);
        return true;
    }
}
