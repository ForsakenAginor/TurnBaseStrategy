using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UpdatableConfiguration<TK, TV> : ScriptableObject
    where TK : Enum
{
    [SerializeField] private List<SerializedPair<TK, TV>> _content;

    protected IReadOnlyList<SerializedPair<TK, TV>> Content => _content;

    private void OnValidate()
    {
        UpdateContent(_content);
        OnEndValidate();
    }

    protected virtual void OnEndValidate()
    {
    }

    protected void UpdateContent<T1, T2>(List<SerializedPair<T1, T2>> content)
    {
        foreach (SerializedPair<T1, T2> pair in Create<T1, T2>())
        {
            if (content.Exists(item => Equals(item.Key, pair.Key)) == true)
                continue;

            content.Add(pair);
        }
    }

    private List<SerializedPair<T1, T2>> Create<T1, T2>()
    {
        List<SerializedPair<T1, T2>> result = new();
        T2 value = default;

        foreach (T1 type in Enum.GetValues(typeof(T1)))
            result.Add(new SerializedPair<T1, T2>(type, value));

        return result;
    }
}

public enum UnitType
{
    Infantry,
    Spearman,
    Archer,
    Knight,
}

public enum CitySize
{
    Village,
    SmallSettlement,
    LargeSettlement,
    City,
}

public class UnitInfo
{
    private readonly int _attack;
    private readonly int _counterAttack;
    private readonly int _health;
    private readonly int _steps;
    private readonly int _cost;

    public UnitInfo(int attack, int counterAttack, int health, int steps, int cost)
    {
        _attack = attack >= 0 ? attack : throw new ArgumentOutOfRangeException(nameof(attack));
        _counterAttack = counterAttack >= 0 ? counterAttack : throw new ArgumentOutOfRangeException(nameof(counterAttack));
        _health = health > 0 ? health : throw new ArgumentOutOfRangeException(nameof(health));
        _steps = steps >= 0 ? steps : throw new ArgumentOutOfRangeException(nameof(steps));
        _cost = cost >= 0 ? cost : throw new ArgumentOutOfRangeException(nameof(cost));
    }

    public int Attack => _attack;

    public int CounterAttack => _counterAttack;

    public int Health => _health;

    public int Steps => _steps;

    public int Cost => _cost;
}

[CreateAssetMenu(fileName = "UnitConfiguration")]
public class UnitsConfiguration : UpdatableConfiguration<UnitType, UnitInfo>, IUnitCostGetter, IUnitInfoGetter
{
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

public interface IUnitCostGetter
{
    public int GetUnitCost(UnitType type);
}

public interface IUnitInfoGetter
{
    public (int attack, int counterAttack, int health, int steps) GetUnitInfo(UnitType type); 
}