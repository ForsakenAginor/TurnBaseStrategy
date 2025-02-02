using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _movingTimePerCell;
    [SerializeField] private Transform _model;
    [SerializeField] private UnitAnimationController _controller;
    [SerializeField] private UnitSoundsHandler _soundHandler;

    public void Move(IEnumerable<Vector3> target, Action onComleteCallback)
    {
        if (target.Count() == 1)
        {
            _model.transform.LookAt(target.First());
            transform.DOMove(target.First(), _movingTimePerCell).SetEase(Ease.Linear).OnComplete(() => { onComleteCallback.Invoke(); _controller.Stop(); });
            _controller.Walk();
            _soundHandler.Walk();
        }
        else if (target.Count() == 2)
        {
            _model.transform.LookAt(target.First());
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOMove(target.First(), _movingTimePerCell).SetEase(Ease.Linear).OnComplete(() => _model.transform.LookAt(target.Last())));
            mySequence.Append(transform.DOMove(target.Last(), _movingTimePerCell).SetEase(Ease.Linear));
            mySequence.AppendCallback(() => { onComleteCallback.Invoke(); _controller.Stop(); });
            _controller.Walk();
            _soundHandler.Walk();
        }
    }

    public void MoveFast(IEnumerable<Vector3> target, Action onComleteCallback)
    {
        float duration = 0.05f;
        if (target.Count() == 1)
        {
            _model.transform.LookAt(target.First());
            transform.DOMove(target.First(), duration).SetEase(Ease.Linear).OnComplete(() => { onComleteCallback.Invoke(); _controller.Stop(); });
            _controller.Walk();
        }
        else if (target.Count() == 2)
        {
            _model.transform.LookAt(target.First());
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOMove(target.First(), duration).SetEase(Ease.Linear).OnComplete(() => _model.transform.LookAt(target.Last())));
            mySequence.Append(transform.DOMove(target.Last(), duration).SetEase(Ease.Linear));
            mySequence.AppendCallback(() => { onComleteCallback.Invoke(); _controller.Stop(); });
            _controller.Walk();
        }
    }
}