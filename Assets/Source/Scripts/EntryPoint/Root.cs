using Assets.Scripts.HexGrid;
using System;
using UnityEngine;

public class Root : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private CellHighlighter _cellHighlighter;
    [SerializeField] private PathHighlighter _pathHighlighter;
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;


    [Header("Player")]
    [SerializeField] private Mover _mover;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);

        _cellHighlighter.Init(_gridCreator.HexGrid, _gridRaycaster);
        _pathHighlighter.Init(_gridCreator.HexGrid, _cellSelector, _gridCreator.PathFinder);
        _pathHighlighter.SetRootCell(new Vector2Int(5, 5));
        _pathHighlighter.EnableControl();
        //_mover.Init(_gridCreator.PathFinder);
        //PlayerMovementManager _ = new PlayerMovementManager(_cellSelector);

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
    }
}

public class UnitSelector : IControllable
{
    private readonly CellSelector _cellSelector;
    private readonly HexGridXZ<WalkableUnit> _grid;
    private bool _isActive = false;

    public UnitSelector(CellSelector cellSelector, HexGridXZ<WalkableUnit> grid)
    {
        _cellSelector = cellSelector != null ? cellSelector : throw new ArgumentNullException(nameof(cellSelector));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
    }

    public event Action<WalkableUnit> UnitSelected;

    public void DisableControl()
    {
        if (_isActive == false)
            return;

        _cellSelector.CellClicked -= OnCellClicked;
        _isActive = false;
    }

    public void EnableControl()
    {
        if (_isActive)
            return; 
        
        _cellSelector.CellClicked += OnCellClicked;
        _isActive = true;
    }

    private void OnCellClicked(Vector3 position, Vector2Int _)
    {
        var unit = _grid.GetGridObject(position);

        if (unit != null)
            UnitSelected?.Invoke(unit);
    }
}