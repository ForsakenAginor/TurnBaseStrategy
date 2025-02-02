using System;
using System.Linq;
using UnityEngine;

public class WinLoseMonitor : MonoBehaviour, IWinLoseEventThrower
{
    private CitiesActionsManager _actionsManager;
    private SaveLevelSystem _saveSystem;
    private GameLevel _gameLevel;
    private bool _isCheckNeeded;

    public event Action PlayerWon;
    public event Action PlayerLost;

    private void LateUpdate()
    {
        if (_isCheckNeeded == false)
            return;

        _isCheckNeeded = false;

        if(_actionsManager.Cities.Any(o => o == Side.Enemy) && _actionsManager.Cities.Any(o => o == Side.Player) == false)
        {
            PlayerLost?.Invoke();
        }
        else if (_actionsManager.Cities.Any(o => o == Side.Enemy) == false && _actionsManager.Cities.Any(o => o == Side.Player))
        {
            GameLevel nextLevel;

            if ((int)_gameLevel == Enum.GetNames(typeof(GameLevel)).Length - 1)
                nextLevel = 0;
            else
                nextLevel = _gameLevel + 1;

            _saveSystem.SaveLevel(nextLevel);
            PlayerWon?.Invoke();
        }
    }

    private void OnDestroy()
    {
        _actionsManager.CitiesChanged -= OnCitiesChanged;        
    }

    public void Init(CitiesActionsManager actionsManager, SaveLevelSystem saveSystem, GameLevel level)
    {
        _actionsManager = actionsManager != null ? actionsManager : throw new ArgumentNullException(nameof(actionsManager));
        _saveSystem = saveSystem != null ? saveSystem : throw new ArgumentNullException(nameof(saveSystem));
        _gameLevel = level;

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
