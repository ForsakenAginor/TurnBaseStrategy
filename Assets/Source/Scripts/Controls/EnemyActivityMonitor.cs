using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivityMonitor : IControllable, IEnemyActivityMonitor
{
    private readonly UnitsActionsManager _actionsManager;
    private readonly List<Vector2Int> _storedEnemyActivity = new();

    public EnemyActivityMonitor(UnitsActionsManager actionsManager)
    {
        _actionsManager = actionsManager != null ? actionsManager : throw new ArgumentNullException(nameof(actionsManager));

        _actionsManager.EnemyDoSomething += OnEnemyDoSomething;
    }

    ~EnemyActivityMonitor()
    {
        _actionsManager.EnemyDoSomething -= OnEnemyDoSomething;
    }

    public event Action<IEnumerable<Vector2Int>> TurnBegun;

    public void DisableControl()
    {

    }

    public void EnableControl()
    {
        TurnBegun?.Invoke(_storedEnemyActivity.ToArray());
        _storedEnemyActivity.Clear();
    }

    private void OnEnemyDoSomething(Vector2Int cell)
    {
        if (_storedEnemyActivity.Contains(cell) == false)
            _storedEnemyActivity.Add(cell);
    }
}

public interface IEnemyActivityMonitor
{
    public event Action<IEnumerable<Vector2Int>> TurnBegun;
}

public interface IEnemyActivityMonitorOrderReceiver
{
    public event Action<Vector2Int> ShowEnemyActivityOrderReceived;
}