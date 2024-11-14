using Assets.Scripts.HexGrid;
using UnityEngine;

public class Root : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private CellHighlighter _cellHighlighter;
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;


    [Header("Player")]
    [SerializeField] private Mover _mover;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellHighlighter.Init(_gridCreator.HexGrid, _gridRaycaster);
        _mover.Init(_gridCreator.PathFinder);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        PlayerMovementManager _ = new PlayerMovementManager(_cellSelector, _mover);

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
    }
}
