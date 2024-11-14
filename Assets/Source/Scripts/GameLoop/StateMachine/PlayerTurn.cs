using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurn : State
{
    private Button _nextTurnButton;

    public PlayerTurn(Button nextTurnButton,Transition[] transitions) : base(transitions)
    {
        _nextTurnButton = nextTurnButton != null ?
            nextTurnButton : 
            throw new ArgumentNullException(nameof(nextTurnButton));

        _nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
    }

    ~PlayerTurn()
    {
        _nextTurnButton.onClick.RemoveListener(OnNextTurnButtonClick);
    }

    public override void DoThing()
    {
        throw new System.NotImplementedException();
    }

    private void OnNextTurnButtonClick()
    {
        Transitions.First(o => o is ToEnemyTurnTransition).SetIsReady(true);
        CallBecomeReadyToTransitEvent();
    }
}

public class GameStateMachine
{
    private State _state;

    public event Action PlayerWon;
    public event Action PlayerLose;

    public GameStateMachine(State state)
    {
        _state = state != null ? state : throw new System.ArgumentNullException(nameof(state));
    }

    private void SetState(Transition transition)
    {
        _state.BecomeReadyToTransit -= OnStateBecomeReadyToTransit;
        transition.SetIsReady(false);
        _state = transition.TargetState;
        _state.BecomeReadyToTransit += OnStateBecomeReadyToTransit;
        _state.DoThing();

        if (_state is PlayerWon)
            PlayerWon?.Invoke();

        if (_state is PlayerLose)
            PlayerLose?.Invoke();
    }

    private void OnStateBecomeReadyToTransit()
    {
        foreach (Transition transition in _state.Transitions)
            if (transition.IsReadyToTransit)
                SetState(transition);
    }
}

public class GameStateMachineCreator : MonoBehaviour
{
    [SerializeField] private Button _nextTurnButton;
    [SerializeField] private UIElement _winScreen;
    [SerializeField] private UIElement _loseScreen;

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
            new Transition[]
            {
                toEnemyTurnTransition,toLoseTransition,toWinTransition
            });

        EnemyTurn enemyTurn = new EnemyTurn(
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

public class EnemyTurn : State
{
    public EnemyTurn(Transition[] transitions) : base(transitions)
    {
    }

    public override void DoThing()
    {
        Transitions.First(o => o is ToPlayerTurnTransition).SetIsReady(true);
        CallBecomeReadyToTransitEvent();
    }
}

public class PlayerWon : State
{
    private IUIElement _winScreen;

    public PlayerWon(IUIElement winScreen) : base(Array.Empty<Transition>())
    {
        _winScreen = winScreen != null ? winScreen : throw new ArgumentNullException(nameof(winScreen));
    }

    public override void DoThing()
    {
        _winScreen.Enable();
    }
}

public class PlayerLose : State
{
    private IUIElement _screen;

    public PlayerLose(IUIElement loseScreen) : base(Array.Empty<Transition>())
    {
        _screen = loseScreen != null ? loseScreen : throw new ArgumentNullException(nameof(loseScreen));
    }

    public override void DoThing()
    {
        _screen.Enable();
    }
}

public class ToEnemyTurnTransition : Transition
{
    public void SetTargetState(EnemyTurn target)
    {
        base.SetTargetState(target);
    }
}

public class ToPlayerTurnTransition : Transition
{
    public void SetTargetState(PlayerTurn target)
    {
        base.SetTargetState(target);
    }
}

public class ToWinTransition : Transition
{
    public void SetTargetState(PlayerWon target)
    {
        base.SetTargetState(target);
    }
}

public class ToLoseTransition : Transition
{
    public void SetTargetState(PlayerLose target)
    {
        base.SetTargetState(target);
    }
}