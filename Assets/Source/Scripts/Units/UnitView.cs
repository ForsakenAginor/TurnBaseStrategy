using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class UnitView : MonoBehaviour, IUIElement
{
    [SerializeField] private TMP_Text _health;
    [SerializeField] private UIElement _viewCanvas;
    [SerializeField] private UnitSoundsHandler _soundHandler;
    [SerializeField] private TMP_Text _healingMessage;
    [SerializeField] private TMP_Text _damagingMessage;
    [SerializeField] private float _animationDuration = 2f;
    [SerializeField] private float _animationDistance = 1f;

    private Vector3 _position;
    private Vector3 _targetPosition;
    private Tween _healingDisplay;
    private Tween _damagingDisplay;
    private Tween _healingFadeoutDisplay;
    private Tween _damagingFadeoutDisplay;
    private Unit _unit;

    private void Awake()
    {
        _position = _healingMessage.transform.localPosition;
        _targetPosition = _position + new Vector3(0, _animationDistance, 0);
    }

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
        _unit.TookDamage += OnTookDamage;
        _unit.Healed += OnHealed;
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

        _unit.TookDamage -= OnTookDamage;
        _unit.Healed -= OnHealed;
        _unit.HealthChanged -= OnHealthChanged;
        _unit.Destroyed -= OnUnitDied;
        DoOnDestroyAction();
    }

    private void OnTookDamage(int value)
    {
        if (_damagingDisplay != null)
        {
            _damagingDisplay.Kill();
            _damagingFadeoutDisplay.Kill();
        }

        _damagingMessage.alpha = 1f;
        _damagingMessage.transform.localPosition = _position;
        _damagingFadeoutDisplay = _damagingMessage.DOFade(0f, _animationDuration).SetEase(Ease.Linear);
        _damagingDisplay = _damagingMessage.transform.DOLocalMove(_targetPosition, _animationDuration)
            .SetEase(Ease.Linear);
        _damagingMessage.text = $"- {value}";
    }

    private void OnHealed(int value)
    {
        if (_healingDisplay != null)
        {
            _healingDisplay.Kill();
            _healingFadeoutDisplay.Kill();
        }

        _healingMessage.alpha = 1f;
        _healingMessage.transform.localPosition = _position;
        _healingFadeoutDisplay = _healingMessage.DOFade(0f, _animationDuration).SetEase(Ease.Linear);
        _healingDisplay = _healingMessage.transform.DOLocalMove(_targetPosition, _animationDuration)
            .SetEase(Ease.Linear);
        _healingMessage.text = $"+ {value}";
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