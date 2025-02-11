using Lean.Localization;
using System;
using TMPro;
using UnityEngine;

public class DayView : MonoBehaviour
{
    [SerializeField] private TMP_Text _dayTextField;
    private DaySystem _daySystem;
    private int _currentDay;

    private void OnDestroy()
    {
        _daySystem.DayChanged -= OnDayChanged;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.U))
            UpdateView();
    }

    public void Init(DaySystem daySystem)
    {
        _daySystem = daySystem != null ? daySystem : throw new ArgumentNullException(nameof(daySystem));
        OnDayChanged(daySystem.CurrentDay);
        
        _daySystem.DayChanged += OnDayChanged;
    }

    private void OnDayChanged(int value)
    {
        _currentDay = value;
        UpdateView();
    }

    private void UpdateView()
    {
        string translation = "Day";
        string day = LeanLocalization.GetTranslationText(translation);
        _dayTextField.text = $"{day} {_currentDay}";
    }
}