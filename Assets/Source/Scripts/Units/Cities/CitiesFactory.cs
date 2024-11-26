using System;

public class CitiesFactory
{
    private readonly ICityBattleInfoGetter _configuration;

    public CitiesFactory(ICityBattleInfoGetter configuration)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
    }

    public CityUnit Create(CitySize size, Side side)
    {
        var tuple = _configuration.GetCityBattleInfo(size);
        Resource health = new(tuple.health);
        return new CityUnit(size, side, health, tuple.counterAttack);
    }
}