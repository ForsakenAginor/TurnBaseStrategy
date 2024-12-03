using System;
using System.Linq;
using UnityEngine;

public class WinLoseMonitor : MonoBehaviour, IWinLoseEventThrower
{
    private CitiesActionsManager _actionsManager;
    private bool _isCheckNeeded;

    public event Action PlayerWon;
    public event Action PlayerLost;

    private void LateUpdate()
    {
        if (_isCheckNeeded == false)
            return;

        _isCheckNeeded = false;

        if(_actionsManager.Cities.Any(o => o == Side.Enemy) && _actionsManager.Cities.Any(o => o == Side.Player) == false)
            PlayerLost?.Invoke();
        else if (_actionsManager.Cities.Any(o => o == Side.Enemy) == false && _actionsManager.Cities.Any(o => o == Side.Player))
            PlayerWon?.Invoke();
    }

    private void OnDestroy()
    {
        _actionsManager.CitiesChanged -= OnCitiesChanged;        
    }

    public void Init(CitiesActionsManager actionsManager)
    {
        _actionsManager = actionsManager != null ? actionsManager : throw new ArgumentNullException(nameof(actionsManager));

        _actionsManager.CitiesChanged += OnCitiesChanged;
    }

    private void OnCitiesChanged()
    {
        _isCheckNeeded = true;   
    }
}

public interface IWinLoseEventThrower
{
    public event Action PlayerWon;
    public event Action PlayerLost;
}
