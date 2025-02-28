using Assets.Scripts.HexGrid;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

public class CameraMover : IControllable
{
    private const float MaxZoomValue = 10f;
    private const float MinZoomValue = 5f;

    private readonly Vector3 _cameraMin;
    private readonly Vector3 _cameraMax;
    private readonly Transform _camera;
    private readonly IZoomInputReceiver _pinchDetector;
    private readonly float _speed = 1f;
    private readonly HexGridXZ<CellSprite> _grid;
    private readonly ICitySearcher _enemyScaner;
    private readonly IEnemyUnitOversight _enemyOversight;
    private readonly ITouchInputReceiver _inputReceiver;
    private readonly ICameraFocusGetter _raycaster;

    private float _cameraYDefault = 5;
    private Tween _tween;
    private Vector2Int _currentFocus;
    private Vector2Int _savedPosition;

    public CameraMover(Transform camera, IZoomInputReceiver pinchDetector,
        GameLevel level, ICameraConfigurationGetter configuration, HexGridXZ<CellSprite> grid, ICitySearcher enemyScaner,
        IEnemyUnitOversight enemyOversight, ITouchInputReceiver inputReceiver, ICameraFocusGetter raycaster, bool isFirstPlayer = true)
    {
        _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
        _pinchDetector = pinchDetector != null ? pinchDetector : throw new ArgumentNullException(nameof(pinchDetector));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        _enemyScaner = enemyScaner != null ? enemyScaner : throw new ArgumentNullException(nameof(enemyScaner));
        _enemyOversight = enemyOversight != null ? enemyOversight : throw new ArgumentNullException(nameof(enemyOversight));
        _inputReceiver = inputReceiver != null ? inputReceiver : throw new ArgumentNullException(nameof(inputReceiver));
        _raycaster = raycaster != null ? raycaster : throw new ArgumentNullException(nameof(raycaster));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _cameraMin = new Vector3(configuration.GetMinimumCameraPosition(level).x, 0, configuration.GetMinimumCameraPosition(level).y);
        _cameraMax = new Vector3(configuration.GetMaximumCameraPosition(level).x, 0, configuration.GetMaximumCameraPosition(level).y);

        var startedCell = isFirstPlayer ? configuration.GetCameraStartPosition(level) : configuration.GetCameraStartPositionSecondPlayer(level);
        FocusCameraOnCell(startedCell);
        _savedPosition = startedCell;
        Subscribe();
    }

    ~CameraMover()
    {
        Unsubscribe();
    }

    public void EnableControl()
    {
        Vector2Int currentPosition = _grid.GetXZ(_raycaster.GetCameraFocus());

        if (currentPosition != _savedPosition)
            FocusCameraOnCell(_savedPosition);            
    }

    public void DisableControl()
    {
        _savedPosition = _grid.GetXZ(_raycaster.GetCameraFocus());
    }

    private void Unsubscribe()
    {
        _inputReceiver.TouchInputReceived -= OnTouchInputReceived;
        _inputReceiver.TouchInputStopped -= OnTouchInputStopped;
        _pinchDetector.GotPinchInput -= OnPinch;
        _enemyScaner.CityFound -= FocusCameraOnCell;
        _enemyOversight.EnemyDoSomething -= FocusCameraOnCell;
    }

    private void Subscribe()
    {
        _inputReceiver.TouchInputStopped += OnTouchInputStopped;
        _inputReceiver.TouchInputReceived += OnTouchInputReceived;
        _pinchDetector.GotPinchInput += OnPinch;
        _enemyScaner.CityFound += FocusCameraOnCell;
        _enemyOversight.EnemyDoSomething += FocusCameraOnCell;
    }

    private void OnTouchInputStopped()
    {
        _currentFocus = _grid.GetXZ(_raycaster.GetCameraFocus());
    }

    private void OnTouchInputReceived(Vector3 input)
    {
        Vector3 target = _camera.position - input;
        target.x = Mathf.Clamp(target.x, _cameraMin.x, _cameraMax.x);
        target.z = Mathf.Clamp(target.z, _cameraMin.z, _cameraMax.z);
        _camera.position = target;
    }

    private void FocusCameraOnCell(Vector2Int cell)
    {
        if (_grid.CashedFarNeighbours[_currentFocus].Contains(cell))
            return;

        _currentFocus = cell;

        if (_tween != null)
            _tween.Kill();

        var cellPosition = _grid.GetCellWorldPosition(cell);
        var cameraPosition = new Vector3(cellPosition.x, _cameraYDefault, cellPosition.z) + _cameraMin;
        _tween = _camera.DOMove(cameraPosition, _speed);
    }

    private void OnPinch(float value)
    {
        Unsubscribe();

        if (_tween != null)
            _tween.Kill();

        if (value < 0)
            _cameraYDefault = MaxZoomValue;
        else
            _cameraYDefault = MinZoomValue;

        Vector3 newPosition = new Vector3(_camera.position.x, _cameraYDefault, _camera.position.z);
        _tween = _camera.DOMove(newPosition, _speed).OnComplete(Subscribe);
    }
}
