using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyActivityMonitorView : MonoBehaviour, IEnemyActivityMonitorOrderReceiver
{
    [SerializeField] private Button _button;

    private IEnemyActivityMonitor _enemyActivityMonitor;
    private List<Vector2Int> _cells;

    public event Action<Vector2Int> ShowEnemyActivityOrderReceived;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClick);
        _enemyActivityMonitor.TurnBegun -= OnTurnBegun;
    }

    public void Init(IEnemyActivityMonitor activityMonitor)
    {
        _enemyActivityMonitor = activityMonitor != null ? activityMonitor : throw new ArgumentNullException(nameof(activityMonitor));

        _enemyActivityMonitor.TurnBegun += OnTurnBegun;
    }

    private void OnTurnBegun(IEnumerable<Vector2Int> enumerable)
    {
        _cells = enumerable.ToList();

        if (_cells.Count > 0)
            _button.gameObject.SetActive(true);
    }

    private void OnButtonClick()
    {
        Vector2Int cell = _cells.Last();
        _cells.Remove(cell);

        if (_cells.Count == 0)
            _button.gameObject.SetActive(false);

        ShowEnemyActivityOrderReceived?.Invoke(cell);
    }
}
