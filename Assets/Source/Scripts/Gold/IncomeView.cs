using System;
using TMPro;
using UnityEngine;

public class IncomeView : MonoBehaviour
{
    [SerializeField] private TMP_Text _incomeTextField;
    private IIncome _income;

    private void OnDestroy()
    {
        _income.IncomeChanged -= OnIncomeChanged;        
    }

    public void Init(IIncome income)
    {
        _income = income != null ? income : throw new ArgumentNullException(nameof(income));
        _income.IncomeChanged += OnIncomeChanged;
    }

    private void OnIncomeChanged(int value)
    {
        if(value >= 0)
        {
            _incomeTextField.color = Color.green;
            _incomeTextField.text = $"+{value}";
        }
        else
        {
            _incomeTextField.color = Color.red;
            _incomeTextField.text = $"{value}";
        }
    }
}