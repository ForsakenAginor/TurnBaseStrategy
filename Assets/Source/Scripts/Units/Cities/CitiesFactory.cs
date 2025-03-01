using System;

public class CitiesFactory
{
    private readonly ICityBattleInfoGetter _configuration;

    public CitiesFactory(ICityBattleInfoGetter configuration)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
    }

    public CityUnit Create(CitySize size, Side side, CityUpgrades upgrades, bool mustCreateWithMaxHealth = true, int currentHealth = int.MinValue)
    {
        var tuple = _configuration.GetCityBattleInfo(size);
        Resource health = mustCreateWithMaxHealth ? new (tuple.health) : new (currentHealth, tuple.health);
        return new CityUnit(size, side, health, tuple.counterAttack, upgrades);
    }
}