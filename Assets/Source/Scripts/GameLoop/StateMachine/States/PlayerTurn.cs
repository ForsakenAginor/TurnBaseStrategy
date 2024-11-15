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
        private readonly List<IResetable> _resetables;
        private readonly IControllable[] _controllables;

        public PlayerTurn(Button nextTurnButton, List<IResetable> resetables,
            IControllable[] controllables, Transition[] transitions)
            : base(transitions)
        {
            _nextTurnButton = nextTurnButton != null ?
                nextTurnButton :
                throw new ArgumentNullException(nameof(nextTurnButton));

            _resetables = resetables != null ?
                resetables :
                throw new ArgumentNullException(nameof(resetables));

            _controllables = controllables != null ?
                controllables :
                throw new ArgumentNullException(nameof(controllables));
        }


        public override void DoThing()
        {
            _nextTurnButton.interactable = true;
            _nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);

            for (int i = 0; i < _resetables.Count; i++)
                _resetables[i].Reset();

            for (int i = 0; i < _controllables.Length; i++)
                _controllables[i].EnableControl();
        }

        private void OnNextTurnButtonClick()
        {
            _nextTurnButton.interactable = false;
            _nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClick);

            for (int i = 0; i < _controllables.Length; i++)
                _controllables[i].DisableControl();

            Transitions.First(o => o is ToEnemyTurnTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }
    }

    public class EnemyTurn : State
    {
        private readonly Button _nextTurnButton;

        public EnemyTurn(Button nextTurnButton, Transition[] transitions) : base(transitions)
        {
            _nextTurnButton = nextTurnButton != null ?
                nextTurnButton :
                throw new ArgumentNullException(nameof(nextTurnButton));
        }

        public override void DoThing()
        {
            _nextTurnButton.interactable = true;
            _nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
        }

        private void OnNextTurnButtonClick()
        {
            _nextTurnButton.interactable = false;
            _nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClick);
            Transitions.First(o => o is ToPlayerTurnTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }
    }

    public class PlayerWon : State
    {
        private readonly IUIElement _winScreen;

        public PlayerWon(IUIElement winScreen) : base(Array.Empty<Transition>())
        {
            _winScreen = winScreen != null ? winScreen : throw new ArgumentNullException(nameof(winScreen));
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