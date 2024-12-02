using DG.Tweening;
using System;
using UnityEngine;

public class CameraMover
{
    private const float MaxZoomValue = 12f;
    private const float MinZoomValue = 5f;
    private const float MinX = 0;
    private const float MinZ = -1;
    private const float MaxX = 10;
    private const float MaxZ = 6;

    private readonly Transform _camera;
    private readonly SwipeHandler _swipeHandler;
    private readonly PinchDetector _pinchDetector;
    private readonly float _zoomMultiplier = 2f;
    private readonly float _speed = 1f;

    private Tween _tween;

    public CameraMover(Transform camera, SwipeHandler swipeHandler, PinchDetector pinchDetector)
    {
        _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
        _swipeHandler = swipeHandler != null ? swipeHandler : throw new ArgumentNullException(nameof(swipeHandler));
        _pinchDetector = pinchDetector != null ? pinchDetector : throw new ArgumentNullException(nameof(pinchDetector));

        _pinchDetector.GotPinchInput += OnPinch;
        _swipeHandler.SwipeInputReceived += OnSwipeInputReceived;
    }

    ~CameraMover()
    {
        _pinchDetector.GotPinchInput -= OnPinch;
        _swipeHandler.SwipeInputReceived -= OnSwipeInputReceived;
    }

    private void OnPinch(float value)
    {
        _tween.Kill();
        Vector3 newPosition = _camera.position + new Vector3(0, value * _zoomMultiplier, 0);
        newPosition.y = Mathf.Clamp(newPosition.y, MinZoomValue, MaxZoomValue);
        _camera.position = newPosition;
    }

    private void OnSwipeInputReceived(Vector3 vector)
    {
        Vector3 target = _camera.position - vector;
        target.x = Mathf.Clamp(target.x, MinX, MaxX);
        target.z = Mathf.Clamp(target.z, MinZ, MaxZ);
        _tween.Kill();
        _tween = _camera.DOMove(target, _speed);
    }
}