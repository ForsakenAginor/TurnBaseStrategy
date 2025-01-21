using System;
using System.Collections;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    [SerializeField] private Transform _model;
    [SerializeField] private UnitAnimationController _controller;
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _timeBeforeHit;
    [SerializeField] private float _timeBeforeEffectPlayed;
    [SerializeField] private UnitSoundsHandler _soundHandler;
    [SerializeField] private ShootingEffect _effect;

    public void Attack(Vector3 target, Action onComleteCallback, Action onDealDamageCallback)
    {
        _model.transform.LookAt(target);
        StartCoroutine(WaitUntilAnimationEnd(target, onComleteCallback, onDealDamageCallback));
        _controller.Attack();
    }

    private IEnumerator WaitUntilAnimationEnd(Vector3 target, Action onComleteCallback, Action onDealDamageCallback)
    {
        WaitForSeconds firstPartDelay = new WaitForSeconds(_timeBeforeEffectPlayed);
        WaitForSeconds secondPartDelay = new WaitForSeconds(_timeBeforeHit - _timeBeforeEffectPlayed);
        WaitForSeconds thirdPartDelay = new WaitForSeconds(_animationDuration - _timeBeforeHit);
        yield return firstPartDelay;
        PlayEffect(target);
        _soundHandler.Attack();
        yield return secondPartDelay;
        onDealDamageCallback?.Invoke();
        yield return thirdPartDelay;
        onComleteCallback?.Invoke();
    }

    private void PlayEffect(Vector3 target)
    {
        if (_effect == null)
            return;

        _effect.PlayEffect(transform.position, target);
    }
}