using System;
using UnityEngine;

[Serializable]
public class UnitInfo
{
    [SerializeField] private int _attack;
    [SerializeField] private int _counterAttack;
    [SerializeField] private int _health;
    [SerializeField] private int _steps;
    [SerializeField] private int _cost;
    [SerializeField] private UnitFacade _prefab;

    public UnitInfo(int attack, int counterAttack, int health, int steps, int cost, UnitFacade prefab)
    {
        _attack = attack >= 0 ? attack : throw new ArgumentOutOfRangeException(nameof(attack));
        _counterAttack = counterAttack >= 0 ? counterAttack : throw new ArgumentOutOfRangeException(nameof(counterAttack));
        _health = health > 0 ? health : throw new ArgumentOutOfRangeException(nameof(health));
        _steps = steps >= 0 ? steps : throw new ArgumentOutOfRangeException(nameof(steps));
        _cost = cost >= 0 ? cost : throw new ArgumentOutOfRangeException(nameof(cost));
        _prefab = prefab != null ? prefab : throw new ArgumentNullException(nameof(prefab));
    }

    public int Attack => _attack;

    public int CounterAttack => _counterAttack;

    public int Health => _health;

    public int Steps => _steps;

    public int Cost => _cost;

    public UnitFacade Prefab => _prefab;
}
