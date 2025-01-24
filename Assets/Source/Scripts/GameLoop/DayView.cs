using TMPro;
using UnityEngine;

public class DayView : MonoBehaviour, IDayView
{
    [SerializeField] private TMP_Text _dayTextField;

    public void ShowCurrentDay(int day)
    {
        _dayTextField.text = $"Day {day}";
    }
}   

public interface IDayView
{
    public void ShowCurrentDay(int day);
}