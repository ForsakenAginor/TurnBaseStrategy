using Sirenix.OdinInspector;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WalkableUnitView : UnitView
{
    [SerializeField] private TMP_Text _attack;
    [SerializeField] private SwitchableElement _oneStep;
    [SerializeField] private SwitchableElement _twoStep;
    [SerializeField] private UnitAnimationController _unitController;
    [SerializeField] private float _timeToDie;
    [SerializeField] private PlayerUnitOnDeathEffect _onDeathEffect;

    private WalkableUnit _unit;
    private int _remainingSteps;

    public override void Init(Unit unit, Action<AudioSource> callback)
    {
        base.Init(unit, callback);

        if (unit is WalkableUnit == false)
            throw new ArgumentException("Wrong Type of unit");

        _unit = unit as WalkableUnit;
        _attack.text = _unit.AttackPower.ToString();
        _remainingSteps = _unit.RemainingSteps;
        ShowRemainingSteps();

        _unit.Moved += OnUnitMoved;
    }

    protected override void DoOnDestroyAction()
    {
        _unit.Moved -= OnUnitMoved;
    }

    protected override void DoOnUnitDiedAction()
    {
        if (_onDeathEffect != null)
            _onDeathEffect.Enable();

        StartCoroutine(StartDying());
    }

#if UNITY_EDITOR
    [Button]
    private void TestDying()
    {
        DoOnUnitDiedAction();
    }
#endif

    private void OnUnitMoved()
    {
        _remainingSteps = _unit.RemainingSteps;
        ShowRemainingSteps();
    }

    private void ShowRemainingSteps()
    {
        if (_oneStep == null || _twoStep == null)
            return;

        switch (_remainingSteps)
        {
            case 0:
                _oneStep.Disable();
                _twoStep.Disable();
                break;
            case 1:
                _oneStep.Enable();
                _twoStep.Disable();
                break;
            case 2:
                _oneStep.Enable();
                _twoStep.Enable();
                break;
            default:
                throw new Exception("Can't display remaining steps");
                break;
        }
    }

    private IEnumerator StartDying()
    {
        WaitForSeconds animationDelay = new WaitForSeconds(_timeToDie);
        _unitController.Die();
        yield return animationDelay;

        if (_onDeathEffect != null)
            _onDeathEffect.Disable();

        gameObject.SetActive(false);
        Destroy(gameObject, _timeToDie);
    }
}
