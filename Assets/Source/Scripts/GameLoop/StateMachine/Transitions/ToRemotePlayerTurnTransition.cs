using Assets.Source.Scripts.GameLoop.StateMachine.States;

namespace Assets.Source.Scripts.GameLoop.StateMachine.Transitions
{
    public class ToRemotePlayerTurnTransition : Transition
    {
        public void SetTargetState(RemotePlayerTurn target)
        {
            base.SetTargetState(target);
        }
    }
}