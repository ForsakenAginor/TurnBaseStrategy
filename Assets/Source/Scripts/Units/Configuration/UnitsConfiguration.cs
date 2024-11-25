using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitConfiguration")]
public class UnitsConfiguration : UpdatableConfiguration<UnitType, UnitInfo>, IUnitCostGetter, IUnitInfoGetter, IUnitPrefabGetter
{
    public UnitFacade GetPrefab(UnitType type)
    {
        return Content.First(o => o.Key == type).Value.Prefab;
    }

    public int GetUnitCost(UnitType type)
    {
        return Content.First(o => o.Key == type).Value.Cost;
    }

    public (int attack, int counterAttack, int health, int steps) GetUnitInfo(UnitType type)
    {
        var value = Content.First(o => o.Key == type).Value;
        return (value.Attack, value.CounterAttack, value.Health, value.Steps);
    }
}
