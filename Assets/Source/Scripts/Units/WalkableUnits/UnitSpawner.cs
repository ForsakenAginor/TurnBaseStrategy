using Assets.Scripts.HexGrid;
using System;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class UnitSpawner : MonoBehaviour, IUnitSpawner
{
    private UnitsConfiguration _configuration;
    private UnitFactory _factory;
    private UnitsActionsManager _unitsManager;
    private HexGridXZ<Unit> _grid;
    private HexGridXZ<IBlockedCell> _landGrid;
    private Resource _wallet;

    public event Action<Unit> UnitSpawned;
    public Action<AudioSource> AudioSourceCallback;

    public void Init(UnitsActionsManager manager, Resource wallet,
        UnitsConfiguration configuration, HexGridXZ<Unit> grid, HexGridXZ<IBlockedCell> landGrid, Action<AudioSource> callback)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _landGrid = landGrid != null ? landGrid : throw new ArgumentNullException(nameof(landGrid));
        AudioSourceCallback = callback != null ? callback : throw new ArgumentNullException(nameof(callback));
        _factory = new UnitFactory(configuration);
    }

    public bool TrySpawnUnit(Vector2Int position, UnitType type, Side side)
    {
        var neighbours = _grid.CashedNeighbours[position].
            Where(o => _grid.IsValidGridPosition(o) && _grid.GetGridObject(o) == null && _landGrid.GetGridObject(o).IsBlocked == false).
            ToList();

        if (neighbours.Count == 0)
            return false;

        int cost = _configuration.GetUnitCost(type);

        if (side == Side.Player)
            if (_wallet.TrySpent(cost) == false)
                return false;

        var unit = _factory.Create(side, type);
        var prefab = side == Side.Enemy ? _configuration.GetEnemyPrefab(type) : _configuration.GetPlayerPrefab(type);
        var facade = Instantiate(prefab, _grid.GetCellWorldPosition(neighbours[0]), Quaternion.identity);
        facade.UnitView.Init(unit, AudioSourceCallback);
        _unitsManager.AddUnit(unit, facade);
        UnitSpawned?.Invoke(unit);

        return true;
    }
}
