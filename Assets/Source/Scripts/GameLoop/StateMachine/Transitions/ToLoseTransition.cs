using Assets.Source.Scripts.GameLoop.StateMachine.States;

namespace Assets.Source.Scripts.GameLoop.StateMachine.Transitions
{
    public class ToLoseTransition : Transition
    {
        public void SetTargetState(PlayerLose target)
        {
            base.SetTargetState(target);
        }
    }
}