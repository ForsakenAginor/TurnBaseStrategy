using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWar : ISavedFogOfWar
{
    private readonly HexGridXZ<ICloud> _fogGrid;
    private readonly HexGridXZ<Unit> _unitGrid;
    private readonly IReadOnlyDictionary<Vector2Int, IEnumerable<Vector2Int>> _neighbours;
    private readonly EnemyScaner _enemyScaner;
    private readonly List<Vector2Int> _discoveredCells = new List<Vector2Int>();

    public FogOfWar(HexGridXZ<ICloud> fogGrid, HexGridXZ<Unit> unitGrid, EnemyScaner enemyScaner)
    {
        _fogGrid = fogGrid != null ? fogGrid : throw new ArgumentNullException(nameof(fogGrid));
        _unitGrid = unitGrid != null ? unitGrid : throw new ArgumentNullException(nameof(unitGrid));
        _enemyScaner = enemyScaner != null ? enemyScaner : throw new ArgumentNullException(nameof(enemyScaner));
        _neighbours = _fogGrid.CashedFarNeighbours;

        _unitGrid.GridObjectChanged += OnGridChanged;
        _enemyScaner.DefendersSpawned += OnDefendersSpawned;
    }

    ~FogOfWar()
    {
        _unitGrid.GridObjectChanged -= OnGridChanged;
        _enemyScaner.DefendersSpawned -= OnDefendersSpawned;
    }

    public IEnumerable<Vector2Int> DiscoveredCells => _discoveredCells.ToList();

    public void ApplyLoadedData(IEnumerable<Vector2Int> discoveredCells)
    {
        DestroyFogAtCells(discoveredCells.ToList());
    }

    private void OnDefendersSpawned(Vector2Int cell)
    {
        DisappearFogOfWar(cell);
    }

    private void OnGridChanged(Vector2Int cell)
    {
        var unit = _unitGrid.GetGridObject(cell);

        if (unit == null || unit.Side == Side.Enemy)
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
