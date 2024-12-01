using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerConfiguration")]
public class EnemySpawnerConfiguration : UpdatableConfiguration<CitySize, UnitType[]>
{
    public IEnumerable<UnitType> GetSpawnedPreset(CitySize citySize) => Content.First(o => o.Key == citySize).Value;
}

[CreateAssetMenu(fileName = "EnemyWaveConfiguration")]
public class EnemyWaveConfiguration : UpdatableConfiguration<CitySize, SerializedPair<int, UnitType[]>>
{
    public IEnumerable<UnitType> GetSpawnedPreset(CitySize citySize) => Content.First(o => o.Key == citySize).Value.Value;

    public int GetSpawnFrequency(CitySize citySize) => Content.First(o => o.Key == citySize).Value.Key;
}
