using System;

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
        return new WalkableUnit(mover, tuple.attack, infantry, side, health, tuple.counterAttack);
    }

    public WalkableUnit CreateKnight(Side side)
    {
        UnitType knight = UnitType.Knight;
        var tuple = _configuration.GetUnitInfo(knight);
        UnitMover mover = new(tuple.steps);
        Resource health = new(tuple.health);
        return new WalkableUnit(mover, tuple.attack, knight, side, health, tuple.counterAttack);
    }
}