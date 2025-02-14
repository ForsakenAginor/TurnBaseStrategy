using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWar : ISavedFogOfWar, IControllable
{
    private readonly HexGridXZ<ICloud> _fogGrid;
    private readonly HexGridXZ<Unit> _unitGrid;
    private readonly IReadOnlyDictionary<Vector2Int, IEnumerable<Vector2Int>> _neighbours;
    private readonly ICitySearcher _enemyScaner;
    private readonly List<Vector2Int> _discoveredCells = new List<Vector2Int>();
    private readonly SwitchableElement _fogHolder;
    private readonly IEnumerable<Side> _enemies;

    public FogOfWar(HexGridXZ<ICloud> fogGrid, HexGridXZ<Unit> unitGrid, ICitySearcher enemyScaner, IEnumerable<Side> enemies)
    {
        _fogGrid = fogGrid != null ? fogGrid : throw new ArgumentNullException(nameof(fogGrid));
        _unitGrid = unitGrid != null ? unitGrid : throw new ArgumentNullException(nameof(unitGrid));
        _enemyScaner = enemyScaner != null ? enemyScaner : throw new ArgumentNullException(nameof(enemyScaner));
        _enemies = enemies != null ? enemies : throw new ArgumentNullException(nameof(enemies));
        _neighbours = _fogGrid.CashedFarNeighbours;

        var cloud = fogGrid.GetGridObject(0, 0);
        _fogHolder = cloud.Holder;


        _unitGrid.GridObjectChanged += OnGridChanged;
        _enemyScaner.CityFound += OnDefendersSpawned;
    }

    ~FogOfWar()
    {
        _unitGrid.GridObjectChanged -= OnGridChanged;
        _enemyScaner.CityFound -= OnDefendersSpawned;
    }

    public void EnableControl()
    {
        _fogHolder.Enable();
    }

    public void DisableControl()
    {
        _fogHolder.Disable();
    }

    public IEnumerable<Vector2Int> DiscoveredCells => _discoveredCells.ToList();

    public void ApplyLoadedData(IEnumerable<Vector2Int> discoveredCells)
    {
        DestroyFogAtCells(discoveredCells.ToList());
    }

    public void ApplyDiscoveredCells(IEnumerable<Vector2Int> discoveredCells)
    {
        foreach (var cell in discoveredCells)
            DisappearFogOfWar(cell);
    }

    private void OnDefendersSpawned(Vector2Int cell)
    {
        DisappearFogOfWar(cell);
    }

    private void OnGridChanged(Vector2Int cell)
    {
        var unit = _unitGrid.GetGridObject(cell);

        if (unit == null || _enemies.Contains(unit.Side))
            return;

        DisappearFogOfWar(cell);
    }

    private void DisappearFogOfWar(Vector2Int cell)
    {
        var disappearedCells = _neighbours[cell].ToList();
        disappearedCells.Add(cell);
        disappearedCells = disappearedCells.Where(o => _fogGrid.IsValidGridPosition(o) && _fogGrid.GetGridObject(o) != null).ToList();

        DestroyFogAtCells(disappearedCells);
    }

    private void DestroyFogAtCells(List<Vector2Int> disappearedCells)
    {
        foreach (var item in disappearedCells)
        {
            _discoveredCells.Add(item);
            var cloud = _fogGrid.GetGridObject(item);
            cloud.Disappear();
            _fogGrid.SetGridObject(item.x, item.y, null);
        }
    }

}

public interface ISavedFogOfWar
{
    public IEnumerable<Vector2Int> DiscoveredCells { get; }
}
