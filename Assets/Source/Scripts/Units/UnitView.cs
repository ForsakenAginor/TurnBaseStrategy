using System;
using TMPro;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    [SerializeField] private TMP_Text _health;

    private Unit _unit;

    private void OnDestroy()
    {
        if (_unit == null)
            return;

        _unit.HealthChanged -= OnHealthChanged;
        _unit.Died -= OnUnitDied;
    }
    public virtual void Init(Unit unit)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        _unit = unit;
        _health.text = _unit.Health.ToString();

        _unit.HealthChanged += OnHealthChanged;
        _unit.Died += OnUnitDied;
    }

    private void OnUnitDied(Unit unit)
    {
        gameObject.SetActive(false);
    }

    private void OnHealthChanged()
    {
        _health.text = _unit.Health.ToString();
    }
}