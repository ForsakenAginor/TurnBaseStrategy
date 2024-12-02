using System;
using System.Collections;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    [SerializeField] private Transform _model;
    [SerializeField] private UnitAnimationController _controller;
    [SerializeField] private float _duration;

    public void Attack(Vector3 target, Action onComleteCallback)
    {
        _model.transform.LookAt(target);
        StartCoroutine(WaitUntilAnimationEnd(onComleteCallback));
        _controller.Attack();
    }

    private IEnumerator WaitUntilAnimationEnd(Action callback)
    {
        WaitForSeconds delay = new WaitForSeconds(_duration);
        yield return delay;
        callback?.Invoke();
    }
}