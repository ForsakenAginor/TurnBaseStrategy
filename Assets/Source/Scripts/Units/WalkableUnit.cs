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

    public bool TryMoveToNextPoint()
    {
        if (_remainingSteps == 0)
            return false;

        _remainingSteps--;
        return true;
    }
}
