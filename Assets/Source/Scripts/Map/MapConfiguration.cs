using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class MapConfiguration
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private Tilemap _prefab;
    [SerializeField] private float _cameraMinX;
    [SerializeField] private float _cameraMaxX;
    [SerializeField] private float _cameraMinZ;
    [SerializeField] private float _cameraMaxZ;
    [SerializeField] private SerializedPair<Vector2Int, CitySize>[] _enemyCities;
    [SerializeField] private SerializedPair<Vector2Int, CitySize>[] _playerCities;
    [SerializeField] private Vector3 _cameraStartPosition;
    [SerializeField] private UnitsConfiguration _unitConfiguration;
    [SerializeField] private CitiesConfiguration _cityConfiguration;
    [SerializeField] private EnemySpawnerConfiguration _enemySpawnerConfiguration;
    [SerializeField] private EnemyWaveConfiguration _enemyWaveConfiguration;

    public int Width => _width;

    public int Height => _height;

    public Tilemap Prefab => _prefab;

    public float CameraMinX => _cameraMinX;

    public float CameraMaxX => _cameraMaxX;

    public float CameraMinZ => _cameraMinZ;

    public float CameraMaxZ => _cameraMaxZ;

    public SerializedPair<Vector2Int, CitySize>[] EnemyCities => _enemyCities;

    public SerializedPair<Vector2Int, CitySize>[] PlayerCities => _playerCities;

    public Vector3 CameraStartPosition => _cameraStartPosition;

    public UnitsConfiguration UnitConfiguration => _unitConfiguration;

    public CitiesConfiguration CityConfiguration => _cityConfiguration;

    public EnemySpawnerConfiguration EnemySpawnerConfiguration => _enemySpawnerConfiguration;

    public EnemyWaveConfiguration EnemyWaveConfiguration => _enemyWaveConfiguration;
}
