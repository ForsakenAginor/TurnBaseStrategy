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
    [SerializeField] private int _salary;
    [SerializeField] private UnitFacade _prefab;

    public int Attack => _attack;

    public int CounterAttack => _counterAttack;

    public int Health => _health;

    public int Steps => _steps;

    public int Cost => _cost;

    public int Salary => _salary;

    public UnitFacade Prefab => _prefab;
}