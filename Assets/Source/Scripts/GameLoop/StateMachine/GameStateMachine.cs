using Assets.Source.Scripts.GameLoop.StateMachine.States;
using System;

namespace Assets.Source.Scripts.GameLoop.StateMachine
{
    public class GameStateMachine
    {
        private State _state;

        public event Action PlayerWon;
        public event Action PlayerLose;

        public GameStateMachine(State state)
        {
            _state = state != null ? state : throw new ArgumentNullException(nameof(state));
            _state.DoThing();
            _state.BecomeReadyToTransit += OnStateBecomeReadyToTransit;
        }

        private void SetState(Transition transition)
        {
            _state.BecomeReadyToTransit -= OnStateBecomeReadyToTransit;
            transition.SetIsReady(false);
            _state = transition.TargetState;
            _state.BecomeReadyToTransit += OnStateBecomeReadyToTransit;
            _state.DoThing();

            if (_state is PlayerWon)
                PlayerWon?.Invoke();

            if (_state is PlayerLose)
                PlayerLose?.Invoke();
        }

        private void OnStateBecomeReadyToTransit()
        {
            foreach (Transition transition in _state.Transitions)
                if (transition.IsReadyToTransit)
                    SetState(transition);
        }
    }
}