using Assets.Scripts.HexGrid;
using UnityEngine;

public class Root : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [SerializeField] private float _gridCellSize;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private float _height = 0.2f;

    private void Awake()
    {
        Vector3 position = new Vector3(0, _height, 0);
        HexGridXZ<CellSprite> grid = new(_gridWidth, _gridHeight, _gridCellSize, position);
        _meshUpdater.Init(grid);
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
    }
}
