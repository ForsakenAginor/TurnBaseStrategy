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

    [Header("Clouds")]
    [SerializeField] private float _cloudHeight = 1;
    [SerializeField] private Cloud _cloudPrefab;
    [SerializeField] private Transform _cloudsHolder;

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

    private void Awake()
    {
        _views = _holder.GetComponentsInChildren<HexOnScene>();
    }

    public void Init()
    {
        Vector3 position = new Vector3(0, _height, 0);
        Vector3 cloudPosition = new Vector3(0, _cloudHeight, 0);
        _hexGrid = new(_gridWidth, _gridHeight, _gridCellSize, position);
        _unitsGrid = new(_gridWidth, _gridHeight, _gridCellSize, position);
        _blockedCells = new HexGridXZ<IBlockedCell>(_gridWidth, _gridHeight, _gridCellSize, position);
        _clouds = new HexGridXZ<ICloud>(_gridWidth, _gridHeight, _gridCellSize, cloudPosition);
        _pathFinder = new HexPathFinder(_gridWidth, _gridHeight, _gridCellSize);

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
