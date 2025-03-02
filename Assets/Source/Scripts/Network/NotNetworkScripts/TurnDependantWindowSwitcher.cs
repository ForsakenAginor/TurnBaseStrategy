using System;
using System.Collections.Generic;

public class TurnDependantWindowSwitcher : IControllable
{
    private readonly IEnumerable<SwitchableElement> _enabledWindows;
    private readonly IEnumerable<SwitchableElement> _disabledWindows;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enabledWindows">Windows that will be enabled, when EnableControl called</param>
    /// <param name="disabledWindows">Windows that will be disabled, when EnableControl called</param>
    /// <exception cref="ArgumentNullException"></exception>
    public TurnDependantWindowSwitcher(IEnumerable<SwitchableElement> enabledWindows, IEnumerable<SwitchableElement> disabledWindows)
    {
        _enabledWindows = enabledWindows != null ? enabledWindows : throw new ArgumentNullException(nameof(enabledWindows));
        _disabledWindows = disabledWindows != null ? disabledWindows : throw new ArgumentNullException(nameof(disabledWindows));

        foreach (SwitchableElement element in _enabledWindows)
            element.Disable();

        foreach (SwitchableElement element in _disabledWindows)
            element.Enable();
    }

    public void DisableControl()
    {
        foreach (SwitchableElement element in _enabledWindows)
            element.Disable();

        foreach (SwitchableElement element in _disabledWindows)
            element.Enable();
    }

    public void EnableControl()
    {
        foreach (SwitchableElement element in _enabledWindows)
            element.Enable();

        foreach (SwitchableElement element in _disabledWindows)
            element.Disable();
    }
}
