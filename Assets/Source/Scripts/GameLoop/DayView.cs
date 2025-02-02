using System;
using TMPro;
using UnityEngine;

public class DayView : MonoBehaviour
{
    [SerializeField] private TMP_Text _dayTextField;
    private DaySystem _daySystem;

    private void OnDestroy()
    {
        _daySystem.DayChanged -= OnDayChanged;
    }

    public void Init(DaySystem daySystem)
    {
        _daySystem = daySystem != null ? daySystem : throw new ArgumentNullException(nameof(daySystem));
        OnDayChanged(daySystem.CurrentDay);
        
        _daySystem.DayChanged += OnDayChanged;
    }

    private void OnDayChanged(int value)
    {
        _dayTextField.text = $"Day {value}";
    }
}