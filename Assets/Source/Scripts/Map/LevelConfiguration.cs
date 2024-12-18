﻿using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "LevelConfiguration")]
public class LevelConfiguration : UpdatableConfiguration<GameLevel, MapConfiguration>,
    ICameraConfigurationGetter, IMapInfoGetter, ICityCoordinatesGetter, ICongigurationGetter
{
    public Vector3 GetCameraStartPosition(GameLevel level) => Content.First(o => o.Key == level).Value.CameraStartPosition;

    public SerializedPair<Vector2Int, CitySize>[] GetEnemyCities(GameLevel level) =>
        Content.First(o => o.Key == level).Value.EnemyCities;

    public SerializedPair<Vector2Int, CitySize>[] GetPlayerCities(GameLevel level) =>
        Content.First(o => o.Key == level).Value.PlayerCities;

    public Tilemap GetMapPrefab(GameLevel level) => Content.First(o => o.Key == level).Value.Prefab;

    public UnitsConfiguration GetUnitConfiguration(GameLevel level) => Content.First(o => o.Key == level).Value.UnitConfiguration;

    public CitiesConfiguration GetCityConfiguration(GameLevel level) => Content.First(o => o.Key == level).Value.CityConfiguration;

    public EnemySpawnerConfiguration GetEnemySpawnerConfiguration(GameLevel level) => Content.First(o => o.Key == level).Value.EnemySpawnerConfiguration;

    public EnemyWaveConfiguration GetEnemyWaveConfiguration(GameLevel level) => Content.First(o => o.Key == level).Value.EnemyWaveConfiguration;

    public Vector2Int GetMapSize(GameLevel level)
    {
        int x = Content.First(o => o.Key == level).Value.Width;
        int z = Content.First(o => o.Key == level).Value.Height;
        return new Vector2Int(x, z);
    }

    public Vector2 GetMaximumCameraPosition(GameLevel level)
    {
        float x = Content.First(o => o.Key == level).Value.CameraMaxX;
        float z = Content.First(o => o.Key == level).Value.CameraMaxZ;
        return new Vector2(x, z);
    }

    public Vector2 GetMinimumCameraPosition(GameLevel level)
    {
        float x = Content.First(o => o.Key == level).Value.CameraMinX;
        float z = Content.First(o => o.Key == level).Value.CameraMinZ;
        return new Vector2(x, z);
    }
}
