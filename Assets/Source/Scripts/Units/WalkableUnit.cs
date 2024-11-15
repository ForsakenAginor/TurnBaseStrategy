using System;

public class WalkableUnit : IResetable
{
    private readonly int _maxSteps;
    private int _remainingSteps;

    public WalkableUnit(int maxSteps)
    {
        _maxSteps = maxSteps > 0 ? maxSteps : throw new System.ArgumentOutOfRangeException(nameof(maxSteps));
        _remainingSteps = maxSteps;
    }

    public void Reset()
    {
        _remainingSteps = _maxSteps;
    }

    public int RemainingSteps => _remainingSteps;

    public bool TryMoving(int steps)
    {
        if(steps <= 0)
            throw new ArgumentOutOfRangeException(nameof(steps));

        if (_remainingSteps < steps)
            return false;

        _remainingSteps -= steps;
        return true;
    }
}
