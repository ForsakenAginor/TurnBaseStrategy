using System;
using System.Linq;
using UnityEngine;

public enum CitySize
{
    Village,
    SmallSettlement,
    LargeSettlement,
    City,
}

[Serializable]
public class CityInfo
{
    [SerializeField] private int _counterAttack;
    [SerializeField] private int _health;
    [SerializeField] private int _goldProducement;
    [SerializeField] private int _upgradeCost;
    [SerializeField] private UnitFacade _prefab;

    public int CounterAttack => _counterAttack;

    public int Health => _health;

    public int GoldProducement => _goldProducement;

    public int UpgradeCost => _upgradeCost;

    public UnitFacade Prefab => _prefab;
}


[CreateAssetMenu(fileName = "CitiesConfiguration")]
public class CitiesConfiguration : UpdatableConfiguration<CitySize, CityInfo>, ICityEconomicInfoGetter, ICityPrefabGetter
{
    public UnitFacade GetPrefab(CitySize size)
    {
        return Content.First(o => o.Key == size).Value.Prefab;
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

public interface ICityBattleInfoGetter
{
    public (int counterAttack, int health) GetCityBattleInfo(CitySize size);
}

public interface ICityPrefabGetter : ICityBattleInfoGetter
{
    public UnitFacade GetPrefab(CitySize size);
}

public interface ICityEconomicInfoGetter
{
    public int GetUpgradeCost(CitySize size);

    public int GetGoldIncome(CitySize size);
}