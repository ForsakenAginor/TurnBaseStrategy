using System;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    private readonly Dictionary<Vector2Int, SerializedPair<Sprite, string>> _bosses;
    private readonly ICitySearcher _enemyScaner;

    public Dialogue(Dictionary<Vector2Int, SerializedPair<Sprite, string>> bosses, ICitySearcher enemyScaner)
    {
        _bosses = bosses != null ? bosses : throw new ArgumentNullException(nameof(bosses));
        _enemyScaner = enemyScaner != null ? enemyScaner : throw new ArgumentNullException(nameof(enemyScaner));

        _enemyScaner.CityFound += OnSpawn;
    }

    ~Dialogue()
    {
        _enemyScaner.CityFound -= OnSpawn;
    }

    public event Action<Sprite, string> BossSpawned;

    private void OnSpawn(Vector2Int position)
    {
        if (_bosses.ContainsKey(position) == false)
            return;

        BossSpawned?.Invoke(_bosses[position].Key, _bosses[position].Value);
    }
}
