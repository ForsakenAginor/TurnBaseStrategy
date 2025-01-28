using Assets.Scripts.HexGrid;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

public class CameraMover
{
    private const float MaxZoomValue = 12f;
    private const float MinZoomValue = 5f;
    private const float CameraYDefault = 6;

    private readonly Vector3 _cameraMin;
    private readonly Vector3 _cameraMax;
    private readonly Transform _camera;
    private readonly SwipeHandler _swipeHandler;
    private readonly PinchDetector _pinchDetector;
    private readonly float _zoomMultiplier = 2f;
    private readonly float _speed = 1f;
    private readonly HexGridXZ<CellSprite> _grid;
    private readonly EnemyScaner _enemyScaner;
    private readonly IEnemyUnitOversight _enemyOversight;

    private Tween _tween;
    private Vector2Int _currentFocus;

    public CameraMover(Transform camera, SwipeHandler swipeHandler, PinchDetector pinchDetector,
        GameLevel level, ICameraConfigurationGetter configuration, HexGridXZ<CellSprite> grid, EnemyScaner enemyScaner,
        IEnemyUnitOversight enemyOversight)
    {
        _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
        _swipeHandler = swipeHandler != null ? swipeHandler : throw new ArgumentNullException(nameof(swipeHandler));
        _pinchDetector = pinchDetector != null ? pinchDetector : throw new ArgumentNullException(nameof(pinchDetector));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _enemyScaner = enemyScaner != null ? enemyScaner : throw new ArgumentNullException(nameof(enemyScaner));
        _enemyOversight = enemyOversight != null ? enemyOversight : throw new ArgumentNullException(nameof(enemyOversight));

        if(configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _cameraMin = new Vector3(configuration.GetMinimumCameraPosition(level).x, 0, configuration.GetMinimumCameraPosition(level).y);
        _cameraMax = new Vector3(configuration.GetMaximumCameraPosition(level).x, 0, configuration.GetMaximumCameraPosition(level).y);

        var startedCell = configuration.GetCameraStartPosition(level);
        FocusCameraOnCell(startedCell);

        _pinchDetector.GotPinchInput += OnPinch;
        _swipeHandler.SwipeInputReceived += OnSwipeInputReceived;
        _enemyScaner.DefendersSpawned += FocusCameraOnCell;
        _enemyOversight.EnemyMoved += FocusCameraOnCell;
    }

    ~CameraMover()
    {
        _pinchDetector.GotPinchInput -= OnPinch;
        _swipeHandler.SwipeInputReceived -= OnSwipeInputReceived;
        _enemyScaner.DefendersSpawned -= FocusCameraOnCell;
        _enemyOversight.EnemyMoved -= FocusCameraOnCell;
    }

    private void FocusCameraOnCell(Vector2Int cell)
    {
        if (_grid.CashedFarNeighbours[_currentFocus].Contains(cell))
            return;

        _currentFocus = cell;

        if(_tween != null)
            _tween.Kill();

        var cellPosition = _grid.GetCellWorldPosition(cell);
        var cameraPosition = new Vector3(cellPosition.x, CameraYDefault, cellPosition.z) + _cameraMin;
        _tween = _camera.DOMove(cameraPosition, _speed);
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
        target.x = Mathf.Clamp(target.x, _cameraMin.x, _cameraMax.x);
        target.z = Mathf.Clamp(target.z, _cameraMin.z, _cameraMax.z);
        _tween.Kill();
        _tween = _camera.DOMove(target, _speed);
    }
}