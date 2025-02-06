using Assets.Source.Scripts.GameLoop.StateMachine.States;
using Assets.Source.Scripts.GameLoop.StateMachine.Transitions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.GameLoop.StateMachine
{
    public class GameStateMachineCreator : MonoBehaviour
    {
        [SerializeField] private EnemyBrain _enemyBrain;
        [SerializeField] private Button _nextTurnButton;
        [SerializeField] private SwitchableElement _winScreen;
        [SerializeField] private SwitchableElement _loseScreen;
        [SerializeField] private SwitchableElement _finishScreen;
        [SerializeField] private WinLoseMonitor _winLoseMonitor;

        public GameStateMachine Create(IEnumerable<IResetable> resetables, IEnumerable<IControllable> controllables,
            IWaitAnimation waitAnimation, GameLevel level)
        {
            if(waitAnimation == null)
                throw new System.ArgumentNullException(nameof(waitAnimation));

            if (resetables == null)
                throw new System.ArgumentNullException(nameof(resetables));

            if (controllables == null)
                throw new System.ArgumentNullException(nameof(controllables));

            //transitions
            ToEnemyTurnTransition toEnemyTurnTransition = new ToEnemyTurnTransition();
            ToLoseTransition toLoseTransition = new ToLoseTransition();
            ToWinTransition toWinTransition = new ToWinTransition();
            ToPlayerTurnTransition toPlayerTurnTransition = new ToPlayerTurnTransition();

            //states
            PlayerTurn playerTurn = new PlayerTurn(waitAnimation,
                _nextTurnButton,
                _winLoseMonitor,
                resetables,
                controllables,
                new Transition[]
                {
                toEnemyTurnTransition,toLoseTransition,toWinTransition
                });

            EnemyTurn enemyTurn = new EnemyTurn(
                _enemyBrain,
                _winLoseMonitor,
                new Transition[]
                {
                toPlayerTurnTransition, toLoseTransition, toWinTransition
                });

            PlayerWon playerWon = new PlayerWon(_winScreen, _finishScreen, level);
            PlayerLose playerLose = new PlayerLose(_loseScreen);

            //transitions initialize
            toPlayerTurnTransition.SetTargetState(playerTurn);
            toEnemyTurnTransition.SetTargetState(enemyTurn);
            toLoseTransition.SetTargetState(playerLose);
            toWinTransition.SetTargetState(playerWon);

            //create State machine
            return new GameStateMachine(playerTurn);
        }
    }
}
