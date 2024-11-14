using System;
using System.Collections.Generic;

public abstract class State
{
    private readonly Transition[] _transitions;

    public State(Transition[] transitions)
    {
        _transitions = transitions != null ? transitions : throw new ArgumentNullException(nameof(transitions));
    }

    public event Action BecomeReadyToTransit;

    public IEnumerable<Transition> Transitions => _transitions;

    public abstract void DoThing();

    protected void CallBecomeReadyToTransitEvent()
    {
        BecomeReadyToTransit?.Invoke();
    }
}
