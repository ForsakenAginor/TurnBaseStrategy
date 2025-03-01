using Assets.Scripts.HexGrid;
using System;
using System.Linq;
using UnityEngine;
using static SavedData;

public class UnitSpawner : MonoBehaviour, IUnitSpawner, IPlayerUnitSpawner
{
    private UnitsConfiguration _configuration;
    private UnitFactory _factory;
    private UnitsActionsManager _unitsManager;
    private HexGridXZ<Unit> _grid;
    private HexGridXZ<IHexOnScene> _landGrid;
    private Resource _walletFirstPlayer;
    private Resource _walletSecondPlayer;

    public event Action<Unit, string> UnitSpawned;
    public event Action<UnitView> UnitViewSpawned;

    public Action<AudioSource> AudioSourceCallback;

    public void Init(UnitsActionsManager manager, Resource wallet,
        UnitsConfiguration configuration, HexGridXZ<Unit> grid, HexGridXZ<IHexOnScene> landGrid, Action<AudioSource> callback)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _walletFirstPlayer = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _landGrid = landGrid != null ? landGrid : throw new ArgumentNullException(nameof(landGrid));
        AudioSourceCallback = callback != null ? callback : throw new ArgumentNullException(nameof(callback));
        _factory = new UnitFactory(configuration);
    }

    public void InitHotSit(UnitsActionsManager manager, Resource walletFirstPlayer, Resource walletSecondPlayer,
        UnitsConfiguration configuration, HexGridXZ<Unit> grid, HexGridXZ<IHexOnScene> landGrid, Action<AudioSource> callback)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _walletFirstPlayer = walletFirstPlayer != null ? walletFirstPlayer : throw new ArgumentNullException(nameof(walletFirstPlayer));
        _walletSecondPlayer = walletSecondPlayer != null ? walletSecondPlayer : throw new ArgumentNullException(nameof(walletSecondPlayer));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _landGrid = landGrid != null ? landGrid : throw new ArgumentNullException(nameof(landGrid));
        AudioSourceCallback = callback != null ? callback : throw new ArgumentNullException(nameof(callback));
        _factory = new UnitFactory(configuration);
    }

    public bool TrySpawnUnit(Vector2Int cityPosition, UnitType type, Side side)
    {
        var neighbours = _grid.CashedNeighbours[cityPosition].
            Where(o => _grid.IsValidGridPosition(o) && _grid.GetGridObject(o) == null && _landGrid.GetGridObject(o).IsBlocked == false).
            ToList();

        if (neighbours.Count == 0)
            return false;

        int cost = _configuration.GetUnitCost(type);

        //if normal game
        if (_walletSecondPlayer == null)
        {
            if (side == Side.Player)
                if (_walletFirstPlayer.TrySpent(cost) == false)
                    return false;
        }
        //if hotsit
        else 
        {
            if (side == Side.Player)
            {
                if (_walletFirstPlayer.TrySpent(cost) == false)
                    return false;
            }
            else if(side == Side.Enemy)
            {
                if (_walletSecondPlayer.TrySpent(cost) == false)
                    return false;
            }
        }

        CreateUnit(type, side, neighbours[0]);

        return true;
    }

    public void SpawnLoadedUnits(SerializedPair<Vector2Int, UnitData>[] units)
    {
        foreach (var unit in units)
            CreateUnit(unit.Value.Type, unit.Value.Side, unit.Key, false, unit.Value.Health, unit.Value.Steps, unit.Value.CanAttack);
    }

    private void CreateUnit(UnitType type, Side side, Vector2Int position,
        bool mustBeNewUnit = true, int health = int.MinValue, int steps = int.MinValue, bool canAttack = true)
    {
        var unit = _factory.Create(side, type, mustBeNewUnit, health, steps, canAttack);
        var prefab = side == Side.Enemy ? _configuration.GetEnemyPrefab(type) : _configuration.GetPlayerPrefab(type);
        var facade = Instantiate(prefab, _grid.GetCellWorldPosition(position), Quaternion.identity);
        facade.UnitView.Init(unit, AudioSourceCallback);
        _unitsManager.AddUnit(unit, facade);
        UnitSpawned?.Invoke(unit, null);

        if (side == Side.Player)
            UnitViewSpawned?.Invoke(facade.UnitView);
    }
}
