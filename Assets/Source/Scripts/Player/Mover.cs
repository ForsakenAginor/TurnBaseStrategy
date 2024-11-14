using DG.Tweening;
using HexPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Transform _movableObject;
    [SerializeField] private float _movingTimePerCell;

    private HexPathFinder _pathFinder;
    private List<Vector3> _path;
    private int _index;
    private bool _isMoving;

    public void Init(HexPathFinder pathFinder)
    {
        _pathFinder = pathFinder != null ? pathFinder : throw new ArgumentNullException(nameof(pathFinder));
    }

    public void Move(Vector3 position)
    {
        if (_isMoving == true)
            return;

        _path = _pathFinder.FindPath(_movableObject.position, position);

        if (_path != null)
        {
            _isMoving = true;
            _path = _path.Select(o => new Vector3(o.x, _movableObject.position.y, o.z)).ToList();
        }

        _index = 0;
        StartAnimation();
    }

    private void StartAnimation()
    {
        _index++;

        if (_index != _path.Count)
            _movableObject.DOMove(_path[_index], _movingTimePerCell).SetEase(Ease.Linear).OnComplete(() => StartAnimation());
        else
            _isMoving = false;
    }
}
