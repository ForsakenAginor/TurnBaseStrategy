using System;

public class CityAtMapInitializer
{
    private readonly ICityCoordinatesGetter _configuration;
    private readonly CitySpawner _citySpawner;
    private readonly GameLevel _level;

    public CityAtMapInitializer(GameLevel level, ICityCoordinatesGetter configuration, CitySpawner citySpawner)
    {
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _citySpawner = citySpawner != null ? citySpawner : throw new ArgumentNullException(nameof(citySpawner));
        _level = level;
    }

    public void SpawnPlayerCities()
    {
        var cities = _configuration.GetPlayerCities(_level);

        foreach (var city in cities)
            _citySpawner.SpawnCity(city.Key, city.Value, Side.Player);
    }

    public void SpawnEnemyCities()
    {
        var cities = _configuration.GetEnemyCities(_level);

        foreach (var city in cities)
            _citySpawner.SpawnCity(city.Key, city.Value, Side.Enemy);
    }
}