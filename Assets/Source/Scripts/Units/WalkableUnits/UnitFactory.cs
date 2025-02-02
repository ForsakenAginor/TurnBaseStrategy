using System;

public class UnitFactory
{
    private readonly IUnitInfoGetter _configuration;

    public UnitFactory(IUnitInfoGetter configuration)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
    }

    public WalkableUnit Create(Side side, UnitType type,
        bool mustBeNew = true, int currentHealth = int.MinValue, int steps = int.MinValue, bool canAttack = true)
    {
        var tuple = _configuration.GetUnitInfo(type);
        UnitMover mover = mustBeNew ? new(tuple.steps) : new(steps, tuple.steps);
        Resource health = mustBeNew ? new(tuple.health) : new Resource(currentHealth, tuple.health);
        return new WalkableUnit(mover, tuple.attack, type, side, health, tuple.counterAttack, canAttack);
    }
}