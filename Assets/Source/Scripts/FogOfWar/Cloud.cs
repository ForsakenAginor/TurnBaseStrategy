using Assets.Scripts.HexGrid;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cloud : MonoBehaviour, ICloud
{
    [SerializeField] private float _duration = 0.5f;
    private bool _isDissappeared;

    public bool IsDissappeared => _isDissappeared;

    public void Disappear()
    {
        transform.DOScale(Vector3.zero, _duration).OnComplete(Remove);
        _isDissappeared = true;
    }

    private void Remove()
    {
        Destroy(gameObject, _duration);
    }
}

public interface ICloud
{
    public bool IsDissappeared { get; }

    public void Disappear();
}

public class FogOfWar
{
    private readonly HexGridXZ<ICloud> _fogGrid;
    private readonly HexGridXZ<Unit> _unitGrid;
    private readonly IReadOnlyDictionary<Vector2Int, IEnumerable<Vector2Int>> _neighbours;
    private readonly EnemyScaner _enemyScaner;

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

        foreach (var item in disappearedCells)
        {
            var cloud = _fogGrid.GetGridObject(item);
            cloud.Disappear();
            _fogGrid.SetGridObject(item.x, item.y, null);
        }
    }
}
