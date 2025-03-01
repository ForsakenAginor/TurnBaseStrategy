public interface ICityPrefabGetter : ICityBattleInfoGetter
{
    public CityFacade GetPlayerPrefab(CitySize size);

    public CityFacade GetEnemyPrefab(CitySize size);

    public CityFacade GetNeutralPrefab(CitySize size);
}
