using System;

public class UnitFactory
{
    private readonly IUnitInfoGetter _configuration;

    public UnitFactory(IUnitInfoGetter configuration)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
    }

    public WalkableUnit Create(Side side, UnitType type)
    {
        var tuple = _configuration.GetUnitInfo(type);
        UnitMover mover = new(tuple.steps);
        Resource health = new(tuple.health);
        return new WalkableUnit(mover, tuple.attack, type, side, health, tuple.counterAttack);
    }
}