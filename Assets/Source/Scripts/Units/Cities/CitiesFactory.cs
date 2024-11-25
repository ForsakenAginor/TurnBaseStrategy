using System;

public class CitiesFactory
{
    private readonly ICityBattleInfoGetter _configuration;

    public CitiesFactory(ICityBattleInfoGetter configuration)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
    }

    public Unit CreateVillage(Side side)
    {
        CitySize village = CitySize.Village;
        var tuple = _configuration.GetCityBattleInfo(village);
        Resource health = new(tuple.health);
        return new Unit(side, health, tuple.counterAttack);
    }
}
