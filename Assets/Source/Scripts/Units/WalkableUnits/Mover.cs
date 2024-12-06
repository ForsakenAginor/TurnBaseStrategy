using System;
using DG.Tweening;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _movingTimePerCell;
    [SerializeField] private Transform _model;
    [SerializeField] private UnitAnimationController _controller;
    [SerializeField] private UnitSoundsHandler _soundHandler;

    public void Move(Vector3 target, Action onComleteCallback)
    {
        _model.transform.LookAt(target);
        transform.DOMove(target, _movingTimePerCell).SetEase(Ease.Linear).OnComplete(()=> { onComleteCallback.Invoke(); _controller.Stop(); });
        _controller.Walk();
        _soundHandler.Walk();
    }
}