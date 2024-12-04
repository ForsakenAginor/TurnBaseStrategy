public interface ICongigurationGetter
{
    public UnitsConfiguration GetUnitConfiguration(GameLevel level);

    public CitiesConfiguration GetCityConfiguration(GameLevel level);

    public EnemySpawnerConfiguration GetEnemySpawnerConfiguration(GameLevel level);

    public EnemyWaveConfiguration GetEnemyWaveConfiguration(GameLevel level);
}
