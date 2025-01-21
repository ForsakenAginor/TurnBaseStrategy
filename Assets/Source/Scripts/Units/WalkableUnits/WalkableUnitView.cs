using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WalkableUnitView : UnitView
{
    [SerializeField] private TMP_Text _attack;
    [SerializeField] private TMP_Text _moving;
    [SerializeField] private UnitAnimationController _unitController;
    [SerializeField] private float _timeToDie;

    private WalkableUnit _unit;

    public override void Init(Unit unit, Action<AudioSource> callback)
    {
        base.Init(unit, callback);

        if (unit is WalkableUnit == false)
            throw new ArgumentException("Wrong Type of unit");

        _unit = unit as WalkableUnit;
        _attack.text = _unit.AttackPower.ToString();
        _moving.text = _unit.RemainingSteps.ToString();

        _unit.Moved += OnUnitMoved;
    }

    protected override void DoOnDestroyAction()
    {
        _unit.Moved -= OnUnitMoved;
    }

    protected override void DoOnUnitDiedAction()
    {
        StartCoroutine(StartDying());
    }

    private void OnUnitMoved()
    {
        _moving.text = _unit.RemainingSteps.ToString();
    }

    private IEnumerator StartDying()
    {
        WaitForSeconds animationDelay = new WaitForSeconds(_timeToDie);
        _unitController.Die();
        yield return animationDelay;
        gameObject.SetActive(false);
    }
}
