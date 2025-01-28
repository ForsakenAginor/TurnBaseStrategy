using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitView : MonoBehaviour, IUIElement
{
    [SerializeField] private TMP_Text _health;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private UIElement _viewCanvas;
    [SerializeField] private UnitSoundsHandler _soundHandler;
    [SerializeField] private TMP_Text _healingMessage;
    [SerializeField] private TMP_Text _damagingMessage;
    [SerializeField] private float _animationDuration = 3f;
    [SerializeField] private float _animationDistance = 1f;
    [SerializeField] private UIElement _title;

    private Vector3 _healingPosition;
    private Vector3 _healingTargetPosition;
    private Vector3 _damagingPosition;
    private Vector3 _damagingTargetPosition;
    private Vector3 _startScale = new Vector3(0.4f, 0.4f, 0.4f);
    private Sequence _healingDisplay = DOTween.Sequence();
    private Sequence _damagingDisplay = DOTween.Sequence();
    private Unit _unit;

    private void Awake()
    {
        _healingPosition = _healingMessage.transform.localPosition;
        _damagingPosition = _damagingMessage.transform.localPosition;
        _healingTargetPosition = _healingPosition + new Vector3(-_animationDistance / 2, _animationDistance, 0);
        _damagingTargetPosition = _damagingPosition + new Vector3(_animationDistance / 2, _animationDistance, 0);
    }

    public void Enable()
    {
        _viewCanvas.Enable();
    }

    public void Disable()
    {
        if (_unit.IsAlive && _unit.HealthMaximum != _unit.Health)
            return;

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

        if (callback == null)
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

    public void ShowTitle()
    {
        _title.Enable();
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

    [Button]
    private void TestPopupDisplay()
    {
        OnTookDamage(1);
        OnHealed(1);
    }

    private void OnTookDamage(int value)
    {
        _damagingDisplay.Kill();

        _damagingMessage.alpha = 1f;
        _damagingMessage.transform.localScale = _startScale;
        _damagingMessage.transform.localPosition = _damagingPosition;
        _damagingDisplay.Append(_damagingMessage.DOFade(0f, _animationDuration).SetEase(Ease.InQuint));
        _damagingDisplay.Join(_damagingMessage.transform.DOLocalMove(_damagingTargetPosition, _animationDuration)
            .SetEase(Ease.Linear));
        _damagingDisplay.Join(_damagingMessage.transform.DOScale(Vector3.one, _animationDuration));
        _damagingMessage.text = $"- {value}";
    }

    private void OnHealed(int value)
    {
        _healingDisplay.Kill();

        _healingMessage.alpha = 1f;
        _healingMessage.transform.localScale = _startScale;
        _healingMessage.transform.localPosition = _healingPosition;
        _healingDisplay.Append(_healingMessage.DOFade(0f, _animationDuration ).SetEase(Ease.InQuint));
        _healingDisplay.Join(_healingMessage.transform.DOLocalMove(_healingTargetPosition, _animationDuration)
            .SetEase(Ease.Linear));
        _healingDisplay.Join(_healingMessage.transform.DOScale(Vector3.one, _animationDuration));
        _healingMessage.text = $"+ {value}";
    }

    private void OnUnitDied(Unit _)
    {
        _soundHandler.Dying();
        DoOnUnitDiedAction();
    }

    private void OnHealthChanged()
    {
        if (_unit.IsAlive && _unit.HealthMaximum != _unit.Health)
        {
            _hpBar.gameObject.SetActive(true);
            _hpBar.value = (float)_unit.Health / _unit.HealthMaximum;
            _health.text = _unit.Health.ToString();
        }
        else
        {
            _hpBar.gameObject.SetActive(false);
        }
    }
}