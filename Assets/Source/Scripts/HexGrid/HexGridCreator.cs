using Assets.Scripts.HexGrid;
using HexPathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGridCreator : MonoBehaviour
{
    [SerializeField] private Tilemap _holder;

    [Header("Grid")]
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private float _gridCellSize = 1;
    [SerializeField] private float _height = 0.25f;

    private HexGridXZ<HexOnScene> _hexesOnScene;
    private HexGridXZ<CellSprite> _hexGrid;
    private HexGridXZ<WalkableUnit> _unitsGrid;
    private HexPathFinder _pathFinder;
    private HexOnScene[] _views;

    public HexGridXZ<HexOnScene> HexGridView => _hexesOnScene;

    public HexGridXZ<CellSprite> HexGrid => _hexGrid;

    public HexGridXZ<WalkableUnit> UnitsGrid => _unitsGrid;

    public HexPathFinder PathFinder => _pathFinder;

    private void Awake()
    {
        _views = _holder.GetComponentsInChildren<HexOnScene>();
    }

    public void Init()
    {
        Vector3 position = new Vector3(0, _height, 0);
        _hexGrid = new(_gridWidth, _gridHeight, _gridCellSize, position);
        _unitsGrid = new(_gridWidth, _gridHeight, _gridCellSize, position);
        _hexesOnScene = new HexGridXZ<HexOnScene>(_gridWidth, _gridHeight, _gridCellSize, position);
        _pathFinder = new HexPathFinder(_gridWidth, _gridHeight, _gridCellSize);

        for (int i = 0; i < _views.Length; i++)
            _hexesOnScene.SetGridObject(_views[i].transform.position, _views[i]);

        for (int i = 0; i < _gridWidth; i++)
            for (int j = 0; j < _gridHeight; j++)
                _pathFinder.MakeNodWalkable(new Vector2Int(i, j));
    }
}
