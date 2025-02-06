using System;

public class UnitMover : IResetable
{
    private readonly int _maxSteps;
    private int _remainingSteps;

    public UnitMover(int maxSteps)
    {
        _maxSteps = maxSteps > 0 ? maxSteps : throw new System.ArgumentOutOfRangeException(nameof(maxSteps));
        _remainingSteps = maxSteps;
    }

    public UnitMover(int currentSteps, int maxSteps)
    {
        _maxSteps = maxSteps > 0 ? maxSteps : throw new System.ArgumentOutOfRangeException(nameof(maxSteps));
        _remainingSteps = currentSteps >= 0 && currentSteps <= _maxSteps ? currentSteps : throw new ArgumentOutOfRangeException(nameof(currentSteps));
    }

    public void Reset()
    {
        _remainingSteps = _maxSteps;
    }

    public int RemainingSteps => _remainingSteps;

    public bool TryMoving(int steps)
    {
        if (steps <= 0)
            throw new ArgumentOutOfRangeException(nameof(steps));

        if (_remainingSteps < steps)
            return false;

        _remainingSteps -= steps;
        return true;
    }

    public void SpentAllSteps()
    {
        _remainingSteps = 0;
    }
}
