using Assets.Source.Scripts.GameLoop.StateMachine.States;

namespace Assets.Source.Scripts.GameLoop.StateMachine.Transitions
{
    public class ToEnemyTurnTransition : Transition
    {
        public void SetTargetState(EnemyTurn target)
        {
            base.SetTargetState(target);
        }
    }
}