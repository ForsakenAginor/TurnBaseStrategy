using System;
using TMPro;
using UnityEngine;

public class UnitView : MonoBehaviour, IUIElement
{
    [SerializeField] private TMP_Text _health;
    [SerializeField] private UIElement _viewCanvas;

    private Unit _unit;

    public void Enable()
    {
        _viewCanvas.Enable();
    }

    public void Disable()
    {
        _viewCanvas.Disable();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public virtual void Init(Unit unit)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        _unit = unit;
        _health.text = _unit.Health.ToString();

        _unit.HealthChanged += OnHealthChanged;
        _unit.Destroyed += OnUnitDied;
    }

    protected virtual void DoOnDestroyAction()
    {

    }

    private void Unsubscribe()
    {
        if (_unit == null)
            return;

        _unit.HealthChanged -= OnHealthChanged;
        _unit.Destroyed -= OnUnitDied;
        DoOnDestroyAction();
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