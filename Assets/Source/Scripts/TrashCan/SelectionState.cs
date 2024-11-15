using System;
using System.Linq;

namespace Assets.Source.Scripts.InteractionStateMachine
{
    public class MovingState : State
    {
        public MovingState(Transition[] transitions) : base(transitions)
        {

        }

        public override void DoThing()
        {

        }
    }

    public class SelectionState : State
    {
        private readonly IControllable _cellHighlighter;

        public SelectionState(IControllable cellHighlighter, Transition[] transitions) : base(transitions)
        {
            _cellHighlighter = cellHighlighter != null ? cellHighlighter : throw new ArgumentNullException(nameof(cellHighlighter));
        }

        public override void DoThing()
        {
            _cellHighlighter.EnableControl();
        }

        private void OnUnitSelected(WalkableUnit _)
        {
            _cellHighlighter.DisableControl();
            Transitions.First(o => o is ToMovingStateTransition).SetIsReady(true);
            CallBecomeReadyToTransitEvent();
        }
    }

    public class StateMachine
    {
        private State _state;

        public StateMachine(State state)
        {
            _state = state != null ? state : throw new ArgumentNullException(nameof(state));
        }

        private void SetState(Transition transition)
        {
            _state.BecomeReadyToTransit -= OnStateBecomeReadyToTransit;
            transition.SetIsReady(false);
            _state = transition.TargetState;
            _state.BecomeReadyToTransit += OnStateBecomeReadyToTransit;
            _state.DoThing();
        }

        private void OnStateBecomeReadyToTransit()
        {
            foreach (Transition transition in _state.Transitions)
                if (transition.IsReadyToTransit)
                    SetState(transition);
        }
    }

    public class InteractionStateMachineCreator
    {
        public StateMachine Create(IControllable cellHighlighter)
        {
            if (cellHighlighter == null)
                throw new ArgumentNullException(nameof(cellHighlighter));


            //transitions
            ToMovingStateTransition toMoving = new ToMovingStateTransition();
            ToSelectionStateTransition toSelection = new ToSelectionStateTransition();

            //states
            SelectionState selectionState = new SelectionState(
                cellHighlighter, 
                new Transition[]
                {
                toMoving
                });

            MovingState movingState = new MovingState(
                new Transition[]
                {
                toSelection
                });

            //transitions initialize
            toSelection.SetTargetState(selectionState);
            toMoving.SetTargetState(movingState);

            //create State machine
            return new StateMachine(selectionState);
        }
    }


    public class ToMovingStateTransition : Transition
    {
        public void SetTargetState(MovingState target)
        {
            base.SetTargetState(target);
        }
    }

    public class ToSelectionStateTransition : Transition
    {
        public void SetTargetState(SelectionState target)
        {
            base.SetTargetState(target);
        }
    }
}