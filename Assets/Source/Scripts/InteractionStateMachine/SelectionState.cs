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
        private readonly UnitSelector _selector;

        public SelectionState(IControllable cellHighlighter, UnitSelector unitSelector, Transition[] transitions) : base(transitions)
        {
            _cellHighlighter = cellHighlighter != null ? cellHighlighter : throw new ArgumentNullException(nameof(cellHighlighter));
            _selector = unitSelector != null ? unitSelector : throw new ArgumentNullException(nameof(unitSelector));
        }

        public override void DoThing()
        {
            _selector.EnableControl();
            _cellHighlighter.EnableControl();
            _selector.UnitSelected += OnUnitSelected;
        }

        private void OnUnitSelected(WalkableUnit _)
        {
            _selector.DisableControl();
            _cellHighlighter.DisableControl();
            _selector.UnitSelected -= OnUnitSelected;
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
        public StateMachine Create(IControllable cellHighlighter, UnitSelector unitSelector)
        {
            if (cellHighlighter == null)
                throw new ArgumentNullException(nameof(cellHighlighter));

            if (unitSelector == null)
                throw new ArgumentNullException(nameof(unitSelector));

            //transitions
            ToMovingStateTransition toMoving = new ToMovingStateTransition();
            ToSelectionStateTransition toSelection = new ToSelectionStateTransition();

            //states
            SelectionState selectionState = new SelectionState(
                cellHighlighter, unitSelector,
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