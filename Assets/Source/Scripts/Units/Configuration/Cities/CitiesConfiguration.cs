using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CitiesConfiguration")]
public class CitiesConfiguration : UpdatableConfiguration<CitySize, CityInfo>, ICityEconomicInfoGetter, ICityPrefabGetter
{
    public CityFacade GetPlayerPrefab(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.PlayerPrefab;
    }

    public CityFacade GetEnemyPrefab(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.EnemyPrefab;
    }

    public int GetUpgradeCost(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.UpgradeCost;
    }

    public (int counterAttack, int health) GetCityBattleInfo(CitySize size)
    {
        var value = Content.First(o => o.Key == size).Value;
        return (value.CounterAttack, value.Health);
    }

    public int GetGoldIncome(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.GoldProducement;
    }
}
