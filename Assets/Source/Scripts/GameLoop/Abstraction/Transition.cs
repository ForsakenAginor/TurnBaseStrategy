using System;

public abstract class Transition
{
    private State _targetState;
    private bool _isReadyToTransit;

    public State TargetState => _targetState;

    public bool IsReadyToTransit => _isReadyToTransit;

    public void SetIsReady(bool value)
    {
        _isReadyToTransit = value;
    }

    protected void SetTargetState(State state)
    {
        _targetState = state != null ? state : throw new ArgumentNullException(nameof(state));
    }
}