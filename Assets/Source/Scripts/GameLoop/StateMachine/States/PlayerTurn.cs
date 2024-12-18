using Assets.Source.Scripts.GameLoop.StateMachine.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Assets.Source.Scripts.GameLoop.StateMachine.States
{
    public class PlayerTurn : State
    {
        private readonly Button _nextTurnButton;
        private readonly IEnumerable<IResetable> _resetables;
        private readonly IEnumerable<IControllable> _controllables;
        private readonly IWinLoseEventThrower _winLoseMonitor;

        public PlayerTurn(Button nextTurnButton, IWinLoseEventThrower winLoseMonitor, IEnumerable<IResetable> resetables,
            IEnumerable<IControllable> controllables, Transition[] transitions)
            : base(transitions)
        {
            _nextTurnButton = nextTurnButton != null ?
                nextTurnButton :
                throw new ArgumentNullException(nameof(nextTurnButton));

            _winLoseMonitor = winLoseMonitor != null ?
                winLoseMonitor :
                throw new ArgumentNullException(nameof(winLoseMonitor));

            _resetables = resetables != null ?
                resetables :
                throw new ArgumentNullException(nameof(resetables));

            _controllables = controllables != null ?
                controllables :
                throw new ArgumentNullException(nameof(controllables));

            _winLoseMonitor.PlayerLost += OnPlayerLost;
            _winLoseMonitor.PlayerWon += OnPlayerWon;
        }

        ~PlayerTurn()
        {
            _winLoseMonitor.PlayerLost -= OnPlayerLost;
            _winLoseMonitor.PlayerWon -= OnPlayerWon;
        }

        public override void DoThing()
        {
            _nextTurnButton.interactable = true;
            _nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);

            foreach (var controllable in _controllables)
                controllable.EnableControl();

            foreach (var resetable in _resetables)
                resetable.Reset();
        }

        private void OnPlayerWon()
        {
            Transitions.First(o => o is ToWinTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }

        private void OnPlayerLost()
        {
            Transitions.First(o => o is ToLoseTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }

        private void OnNextTurnButtonClick()
        {
            _nextTurnButton.interactable = false;
            _nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClick);

            foreach (var controllable in _controllables)
                controllable.DisableControl();

            Transitions.First(o => o is ToEnemyTurnTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }
    }

    public class EnemyTurn : State
    {
        private readonly EnemyBrain _enemyBrain;
        private readonly IWinLoseEventThrower _winLoseMonitor;
        private readonly EnemyWaveSpawner _enemyWaveSpawner;

        public EnemyTurn(EnemyBrain enemyBrain, EnemyWaveSpawner waveSpawner, IWinLoseEventThrower winLoseMonitor, Transition[] transitions) : base(transitions)
        {
            _enemyBrain = enemyBrain != null ? enemyBrain : throw new ArgumentNullException(nameof(enemyBrain));
            _enemyWaveSpawner = waveSpawner != null ? waveSpawner : throw new ArgumentNullException(nameof(waveSpawner));
            _winLoseMonitor = winLoseMonitor != null ?
                winLoseMonitor :
                throw new ArgumentNullException(nameof(winLoseMonitor));

            _enemyBrain.TurnEnded += OnTurnEnded;
            _winLoseMonitor.PlayerLost += OnPlayerLost;
            _winLoseMonitor.PlayerWon += OnPlayerWon;
        }

        ~EnemyTurn()
        {
            _enemyBrain.TurnEnded -= OnTurnEnded;
            _winLoseMonitor.PlayerLost -= OnPlayerLost;
            _winLoseMonitor.PlayerWon -= OnPlayerWon;
        }

        public override void DoThing()
        {
            _enemyBrain.EnableControl();
            _enemyWaveSpawner.SpawnWave();
        }

        private void OnPlayerWon()
        {
            Transitions.First(o => o is ToWinTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }

        private void OnPlayerLost()
        {
            Transitions.First(o => o is ToLoseTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }

        private void OnTurnEnded()
        {
            _enemyBrain.DisableControl();
            Transitions.First(o => o is ToPlayerTurnTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }
    }

    public class PlayerWon : State
    {
        private readonly IUIElement _winScreen;

        public PlayerWon(IUIElement winScreen, IUIElement finishScreen, GameLevel level) : base(Array.Empty<Transition>())
        {
            if(winScreen == null)
                throw new ArgumentNullException(nameof(winScreen));

            if(finishScreen == null)
                throw new ArgumentNullException(nameof(finishScreen));

            _winScreen = (int) level == Enum.GetNames(typeof(GameLevel)).Length - 1 ? finishScreen : winScreen;
        }

        public override void DoThing()
        {
            _winScreen.Enable();
        }
    }

    public class PlayerLose : State
    {
        private readonly IUIElement _screen;

        public PlayerLose(IUIElement loseScreen) : base(Array.Empty<Transition>())
        {
            _screen = loseScreen != null ? loseScreen : throw new ArgumentNullException(nameof(loseScreen));
        }

        public override void DoThing()
        {
            _screen.Enable();
        }
    }
}