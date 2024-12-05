using System;
using TMPro;
using UnityEngine;

public class UnitView : MonoBehaviour, IUIElement
{
    [SerializeField] private TMP_Text _health;
    [SerializeField] private UIElement _viewCanvas;
    [SerializeField] private UnitSoundsHandler _soundHandler;

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

    public virtual void Init(Unit unit, Action<AudioSource> callback)
    {
        if (unit == null)
            throw new ArgumentNullException(nameof(unit));

        if(callback == null)
            throw new ArgumentNullException(nameof(callback));

        _unit = unit;
        _soundHandler.Init(callback);
        _soundHandler.Hire();
        _health.text = _unit.Health.ToString();

        _unit.HealthChanged += OnHealthChanged;
        _unit.Destroyed += OnUnitDied;
    }

    protected virtual void DoOnDestroyAction()
    {
    }

    protected virtual void DoOnUnitDiedAction()
    {
        gameObject.SetActive(false);
    }

    private void Unsubscribe()
    {
        if (_unit == null)
            return;

        _unit.HealthChanged -= OnHealthChanged;
        _unit.Destroyed -= OnUnitDied;
        DoOnDestroyAction();
    }

    private void OnUnitDied(Unit _)
    {
        _soundHandler.Dying();
        DoOnUnitDiedAction();
    }

    private void OnHealthChanged()
    {
        _health.text = _unit.Health.ToString();
    }
}