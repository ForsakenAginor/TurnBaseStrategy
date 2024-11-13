using Assets.Scripts.HexGrid;
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
    private HexOnScene[] _views;

    public HexGridXZ<HexOnScene> HexGridView => _hexesOnScene;

    public HexGridXZ<CellSprite> HexGrid => _hexGrid;

    private void Awake()
    {
        _views = _holder.GetComponentsInChildren<HexOnScene>();
    }

    public void Init()
    {
        Vector3 position = new Vector3(0, _height, 0);
        _hexGrid = new(_gridWidth, _gridHeight, _gridCellSize, position);

        _hexesOnScene = new HexGridXZ<HexOnScene>(_gridWidth, _gridHeight, _gridCellSize, position);

        for (int i = 0; i < _views.Length; i++)
            _hexesOnScene.SetGridObject(_views[i].transform.position, _views[i]);
    }
}
