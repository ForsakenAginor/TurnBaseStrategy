using System;
using DG.Tweening;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _movingTimePerCell;
    [SerializeField] private Transform _model;

    public void Move(Vector3 target, Action onComleteCallback)
    {
        _model.transform.LookAt(target);
        transform.DOMove(target, _movingTimePerCell).SetEase(Ease.Linear).OnComplete(()=> onComleteCallback.Invoke());
    }
}