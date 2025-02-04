using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IncomeCompositionView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text _leftTextField;
    [SerializeField] private TMP_Text _rightTextField;
    [SerializeField] private SwitchableElement _tooltip;
    private IIncome _income;

    private void OnDestroy()
    {
        _income.IncomeCompositionChanged -= OnIncomeChanged;
    }

    public void Init(IIncome income)
    {
        _income = income != null ? income : throw new ArgumentNullException(nameof(income));

        _income.IncomeCompositionChanged += OnIncomeChanged;
    }

    private void OnIncomeChanged(List<KeyValuePair<string, int>> list)
    {
        StringBuilder leftBuilder = new StringBuilder();
        StringBuilder rightBuilder = new StringBuilder();

        foreach(var part in list)
        {
            if (part.Value > 0)
                leftBuilder.AppendLine($"<color=#00FF00> +{part.Value} {part.Key}</color>");
            else
                rightBuilder.AppendLine($"<color=#FF0000> {part.Value} {part.Key}</color>");
        }

        _leftTextField.text = leftBuilder.ToString();
        _rightTextField.text = rightBuilder.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.Disable();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltip.Enable();
    }
}
