using System;
using UnityEngine;

[Serializable]
public class CityInfo
{
    [SerializeField] private int _counterAttack;
    [SerializeField] private int _health;
    [SerializeField] private int _goldProducement;
    [SerializeField] private int _upgradeCost;
    [SerializeField] private CityFacade _playerPrefab;
    [SerializeField] private CityFacade _enemyPrefab;

    public int CounterAttack => _counterAttack;

    public int Health => _health;

    public int GoldProducement => _goldProducement;

    public int UpgradeCost => _upgradeCost;

    public CityFacade PlayerPrefab => _playerPrefab;

    public CityFacade EnemyPrefab => _enemyPrefab;
}
