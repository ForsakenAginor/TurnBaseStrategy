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

        public GameStateMachine CreateMultiplayer(PhotonEventReceiver photonEventReceiver, IEnumerable<IResetable> resetables, IEnumerable<IControllable> controllables,
            IWaitAnimation waitAnimation, GameLevel level, bool isFirstPlayer)
        {
            if (waitAnimation == null)
                throw new System.ArgumentNullException(nameof(waitAnimation));

            if (resetables == null)
                throw new System.ArgumentNullException(nameof(resetables));

            if (controllables == null)
                throw new System.ArgumentNullException(nameof(controllables));

            if(photonEventReceiver == null)
                throw new System.ArgumentNullException(nameof(photonEventReceiver));

            //transitions
            ToRemotePlayerTurnTransition toEnemyTurnTransition = new ToRemotePlayerTurnTransition();
            ToLoseTransition toLoseTransition = new ToLoseTransition();
            ToWinTransition toWinTransition = new ToWinTransition();
            ToPlayerTurnTransition toPlayerTurnTransition = new ToPlayerTurnTransition();

            IEnumerable<IResetable> playerResettables;
            IEnumerable<IResetable> remotePlayerResettables;

            if(isFirstPlayer)
            {
                playerResettables = resetables;
                remotePlayerResettables = new List<IResetable>();
            }
            else
            {
                playerResettables = new List<IResetable>();
                remotePlayerResettables = resetables;
                _nextTurnButton.interactable = false;
            }

            //states
            PlayerTurn playerTurn = new PlayerTurn(waitAnimation,
                _nextTurnButton,
                _winLoseMonitor,
                playerResettables,
                controllables,
                new Transition[]
                {
                toEnemyTurnTransition,toLoseTransition,toWinTransition
                });

            RemotePlayerTurn enemyTurn = new RemotePlayerTurn(
                remotePlayerResettables,
                photonEventReceiver,
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
            return isFirstPlayer ? new GameStateMachine(playerTurn) : new GameStateMachine(enemyTurn);
        }

        public GameStateMachine CreateHotSit(IEnumerable<IResetable> resetables,
            IEnumerable<IControllable> controllables1, IWaitAnimation waitAnimation1,
            IEnumerable<IControllable> controllables2, IWaitAnimation waitAnimation2,
            GameLevel level)
        {
            if (waitAnimation1 == null)
                throw new System.ArgumentNullException(nameof(waitAnimation1));

            if (waitAnimation2 == null)
                throw new System.ArgumentNullException(nameof(waitAnimation2));

            if (resetables == null)
                throw new System.ArgumentNullException(nameof(resetables));

            if (controllables1 == null)
                throw new System.ArgumentNullException(nameof(controllables1));

            if (controllables2 == null)
                throw new System.ArgumentNullException(nameof(controllables2));

            //transitions
            ToPlayerTurnTransition toEnemyTurnTransition = new ToPlayerTurnTransition();
            ToLoseTransition toLoseTransition = new ToLoseTransition();
            ToWinTransition toWinTransition = new ToWinTransition();
            ToPlayerTurnTransition toPlayerTurnTransition = new ToPlayerTurnTransition();

            //states
            PlayerTurn playerTurn = new PlayerTurn(waitAnimation1,
                _nextTurnButton,
                _winLoseMonitor,
                resetables,
                controllables1,
                new Transition[]
                {
                toEnemyTurnTransition, toLoseTransition, toWinTransition
                });

            PlayerTurn enemyTurn = new PlayerTurn(waitAnimation2,
                _nextTurnButton,
                _winLoseMonitor,
                new List<IResetable>(),
                controllables2,
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

            //disable controllables
            foreach (var controllable in controllables2)
                controllable.DisableControl();

            //create State machine
            return new GameStateMachine(playerTurn);
        }
    }
}
