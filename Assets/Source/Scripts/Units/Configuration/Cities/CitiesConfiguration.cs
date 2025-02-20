using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CitiesConfiguration")]
public class CitiesConfiguration : UpdatableConfiguration<CitySize, CityInfo>, ICityPrefabGetter
{
    public CityFacade GetPlayerPrefab(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.PlayerPrefab;
    }

    public CityFacade GetEnemyPrefab(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.EnemyPrefab;
    }

    public CityFacade GetNeutralPrefab(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.NeutralPrefab;
    }

    public (int counterAttack, int health) GetCityBattleInfo(CitySize size)
    {
        var value = Content.First(o => o.Key == size).Value;
        return (value.CounterAttack, value.Health);
    }
}
