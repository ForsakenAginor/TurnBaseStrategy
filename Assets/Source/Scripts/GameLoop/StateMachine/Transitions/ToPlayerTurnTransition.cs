using Assets.Source.Scripts.GameLoop.StateMachine.States;

namespace Assets.Source.Scripts.GameLoop.StateMachine.Transitions
{
    public class ToPlayerTurnTransition : Transition
    {
        public void SetTargetState(PlayerTurn target)
        {
            base.SetTargetState(target);
        }
    }
}