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
        [SerializeField] private UIElement _winScreen;
        [SerializeField] private UIElement _loseScreen;
        [SerializeField] private WinLoseMonitor _winLoseMonitor;

        public GameStateMachine Create(IEnumerable<IResetable> resetables, IEnumerable<IControllable> controllables)
        {
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
            PlayerTurn playerTurn = new PlayerTurn(
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

            PlayerWon playerWon = new PlayerWon(_winScreen);
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