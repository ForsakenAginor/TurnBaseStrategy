using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _movingTimePerCell;

    private List<Vector3> _path;
    private int _index;
    private bool _isWorking = false;

    public void Move(List<Vector3> path)
    {
        if (_isWorking)
        {
            _path.AddRange(path);
            return;
        }

        _path = path;
        _isWorking = true;
        _index = 0;
        StartAnimation();
    }

    private void StartAnimation()
    {
        if (_index != _path.Count)
            transform.DOMove(_path[_index], _movingTimePerCell).SetEase(Ease.Linear).OnComplete(() => StartAnimation());
        else
            _isWorking = false;

        _index++;
    }
}