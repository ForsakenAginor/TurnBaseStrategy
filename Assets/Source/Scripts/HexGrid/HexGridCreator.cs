﻿using Assets.Scripts.HexGrid;
using HexPathfinding;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGridCreator : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private float _gridCellSize = 1;
    [SerializeField] private float _height = 0.25f;
    private int _gridWidth = 10;
    private int _gridHeight = 10;

    [Header("Clouds")]
    [SerializeField] private float _cloudHeight = 1;
    [SerializeField] private Cloud _cloudPrefab;
    [SerializeField] private Transform _cloudsHolder;

    [Header("Tilemap")]
    [SerializeField] private Grid _tilemapGrid;
    private Tilemap _tilemapPrefab;

    [Header("Grids")]
    private HexGridXZ<IBlockedCell> _blockedCells;
    private HexGridXZ<CellSprite> _hexGrid;
    private HexGridXZ<Unit> _unitsGrid;
    private HexGridXZ<ICloud> _clouds;
    private HexPathFinder _pathFinder;
    private HexOnScene[] _views;

    public HexGridXZ<IBlockedCell> BlockedCells => _blockedCells;

    public HexGridXZ<CellSprite> HexGrid => _hexGrid;

    public HexGridXZ<Unit> UnitsGrid => _unitsGrid;

    public HexPathFinder PathFinder => _pathFinder;

    public HexGridXZ<ICloud> Clouds => _clouds;

    public void Init(GameLevel level, IMapInfoGetter configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _gridHeight = configuration.GetMapSize(level).y;
        _gridWidth = configuration.GetMapSize(level).x;
        _tilemapPrefab = configuration.GetMapPrefab(level);

        Vector3 position = new Vector3(0, _height, 0);
        Vector3 cloudPosition = new Vector3(0, _cloudHeight, 0);
        _hexGrid = new(_gridWidth, _gridHeight, _gridCellSize, position);
        _unitsGrid = new(_gridWidth, _gridHeight, _gridCellSize, position);
        _blockedCells = new HexGridXZ<IBlockedCell>(_gridWidth, _gridHeight, _gridCellSize, position);
        _clouds = new HexGridXZ<ICloud>(_gridWidth, _gridHeight, _gridCellSize, cloudPosition);
        _pathFinder = new HexPathFinder(_gridWidth, _gridHeight, _gridCellSize);

        Instantiate(_tilemapPrefab, _tilemapGrid.transform);
        _views = _tilemapGrid.GetComponentsInChildren<HexOnScene>();

        for (int i = 0; i < _views.Length; i++)
            _blockedCells.SetGridObject(_views[i].transform.position, _views[i]);

        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                if (_blockedCells.GetGridObject(i, j).IsBlocked)
                    _pathFinder.MakeNodUnwalkable(new Vector2Int(i, j));
                else
                    _pathFinder.MakeNodWalkable(new Vector2Int(i, j));

                var cloud = Instantiate(_cloudPrefab, _clouds.GetCellWorldPosition(i, j), Quaternion.identity);
                cloud.transform.SetParent(_cloudsHolder);
                _clouds.SetGridObject(i, j, cloud);
            }
        }
    }
}
