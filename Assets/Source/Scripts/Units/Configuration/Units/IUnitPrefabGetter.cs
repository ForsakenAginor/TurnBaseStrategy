public interface IUnitPrefabGetter : IUnitInfoGetter
{
    public UnitFacade GetEnemyPrefab(UnitType type);

    public UnitFacade GetPlayerPrefab(UnitType type);
}
