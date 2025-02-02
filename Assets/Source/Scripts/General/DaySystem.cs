using System;

public class DaySystem : IResetable, ISavedDaySystem
{
    private int _currentDay;

    public DaySystem(int currentDay = 1)
    {
        _currentDay = currentDay;
    }

    public event Action<int> DayChanged;

    public int CurrentDay => _currentDay;

    public void Reset()
    {
        _currentDay++;
        DayChanged?.Invoke(_currentDay);
    }
}

public interface ISavedDaySystem
{
    public int CurrentDay { get; }
}
