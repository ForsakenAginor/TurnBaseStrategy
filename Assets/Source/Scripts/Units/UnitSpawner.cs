using Assets.Scripts.HexGrid;
using System;
using System.Linq;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    private IUnitPrefabGetter _configuration;
    private UnitFactory _factory;
    private UnitsActionsManager _unitsManager;
    private HexGridXZ<Unit> _grid;
    private HexGridXZ<IBlockedCell> _landGrid;

    public void Init(UnitsActionsManager manager, IUnitPrefabGetter configuration, HexGridXZ<Unit> grid, HexGridXZ<IBlockedCell> landGrid)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _landGrid = landGrid != null ? landGrid : throw new ArgumentNullException(nameof(landGrid));
        _factory = new UnitFactory(configuration);
    }

    public bool TrySpawnUnit(Vector2Int position, UnitType type, Side side)
    {
        var neighbours = _grid.CashedNeighbours[position].
            Where(o => _grid.IsValidGridPosition(o) && _grid.GetGridObject(o) == null && _landGrid.GetGridObject(o).IsBlocked == false).
            ToList();

        if(neighbours.Count == 0)
            return false;
        
        switch(type)
        {
            case UnitType.Infantry:
                var unit = _factory.CreateInfantry(side);
                var facade = Instantiate(_configuration.GetPrefab(type), _grid.GetCellWorldPosition(neighbours[0]), Quaternion.identity);
                facade.UnitView.Init(unit);
                _unitsManager.AddUnit(unit, facade);
                break;
            default:
                break;
        }

        return true;
    }
}
