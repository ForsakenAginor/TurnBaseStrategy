using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateMachineCreator : MonoBehaviour
{
    [SerializeField] private Button _nextTurnButton;
    [SerializeField] private UIElement _winScreen;
    [SerializeField] private UIElement _loseScreen;

    [Header("Debug")]
    [SerializeField] private Button _enemyTurnSkipButton;

    private readonly List<IControllable> _controllables = new ();
    private readonly List<IResetable> _resetables = new ();

    public GameStateMachine Create()
    {
        //transitions
        ToEnemyTurnTransition toEnemyTurnTransition = new ToEnemyTurnTransition();
        ToLoseTransition toLoseTransition = new ToLoseTransition();
        ToWinTransition toWinTransition = new ToWinTransition();
        ToPlayerTurnTransition toPlayerTurnTransition = new ToPlayerTurnTransition();

        //states
        PlayerTurn playerTurn = new PlayerTurn(
            _nextTurnButton,
            _resetables.ToArray(),
            _controllables.ToArray(),
            new Transition[]
            {
                toEnemyTurnTransition,toLoseTransition,toWinTransition
            });

        EnemyTurn enemyTurn = new EnemyTurn(
            _enemyTurnSkipButton,
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
