using Assets.Source.Scripts.GameLoop.StateMachine.States;

namespace Assets.Source.Scripts.GameLoop.StateMachine.Transitions
{
    public class ToWinTransition : Transition
    {
        public void SetTargetState(PlayerWon target)
        {
            base.SetTargetState(target);
        }
    }
}