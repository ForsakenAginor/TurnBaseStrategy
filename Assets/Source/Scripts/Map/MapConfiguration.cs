using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class MapConfiguration
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private Tilemap _prefab;
    [SerializeField] private SerializedPair<Vector2Int, CitySize>[] _enemyCities;
    [SerializeField] private SerializedPair<Vector2Int, CitySize>[] _playerCities;
    [SerializeField] private SerializedPair<Vector2Int, CitySize>[] _neutralCities;
    [SerializeField] private SerializedPair<Vector2Int, string>[] _citiesNames;
    [SerializeField] private SerializedPair<Vector2Int, SerializedPair<Sprite, string>>[] _spawnMessages;
    [SerializeField] private Vector2Int _cameraStartPosition;
    [SerializeField] private Vector2Int _cameraStartPositionSecondPlayer;
    [SerializeField] private UnitsConfiguration _unitConfiguration;
    [SerializeField] private CitiesConfiguration _cityConfiguration;
    [SerializeField] private EnemySpawnerConfiguration _enemySpawnerConfiguration;
    [SerializeField] private EnemyWaveConfiguration _enemyWaveConfiguration;
    [SerializeField] private Vector2 _cameraMin = new (0, -2);
    [SerializeField] private CityUpgradesCostConfiguration _cityUpgradesCostConfiguration;

    public int Width => _width;

    public int Height => _height;

    public Tilemap Prefab => _prefab;

    public Vector2 CameraMin => _cameraMin;

    public Vector2 CameraMax => _cameraMin + new Vector2(_width, _height * 0.75f);

    public SerializedPair<Vector2Int, CitySize>[] EnemyCities => _enemyCities;

    public SerializedPair<Vector2Int, CitySize>[] PlayerCities => _playerCities;

    public SerializedPair<Vector2Int, CitySize>[] NeutralCities => _neutralCities;

    public SerializedPair<Vector2Int, string>[] CitiesNames => _citiesNames;

    public SerializedPair<Vector2Int, SerializedPair<Sprite, string>>[] SpawnMessages => _spawnMessages;

    public Vector2Int CameraStartPosition => _cameraStartPosition;

    public Vector2Int CameraStartPositionSecondPlayer => _cameraStartPositionSecondPlayer;

    public UnitsConfiguration UnitConfiguration => _unitConfiguration;

    public CitiesConfiguration CityConfiguration => _cityConfiguration;

    public EnemySpawnerConfiguration EnemySpawnerConfiguration => _enemySpawnerConfiguration;

    public EnemyWaveConfiguration EnemyWaveConfiguration => _enemyWaveConfiguration;

    public ICityUpgradesCostConfiguration CityUpgradesCostConfiguration => _cityUpgradesCostConfiguration; 
}
