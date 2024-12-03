using Assets.Scripts.HexGrid;
using Lean.Touch;
using System;
using UnityEngine;

public class CellSelector : MonoBehaviour
{
    [SerializeField] private LeanFingerDown _fingerDown;

    private HexGridXZ<CellSprite> _hexGrid;
    private GridRaycaster _gridRaycaster;
    private bool _isWorking = false;

    public event Action<Vector3, Vector2Int> CellClicked;

    private void Start()
    {
        _fingerDown.OnFinger.AddListener(OnFingerDown);
    }

    private void OnDestroy()
    {
        _fingerDown.OnFinger.RemoveListener(OnFingerDown);
    }

    private void OnFingerDown(LeanFinger finger)
    {
        if (_isWorking == false)
            return;

        if (finger.Down && _gridRaycaster.TryGetPointerPosition(finger.ScreenPosition, out Vector3 worldPosition))
        {
            var position = _hexGrid.GetXZ(worldPosition);

            if (_hexGrid.IsValidGridPosition(position))
                CellClicked?.Invoke(worldPosition, position);
        }
    }

    public void Init(HexGridXZ<CellSprite> hexGrid, GridRaycaster gridRaycaster)
    {
        _hexGrid = hexGrid != null ? hexGrid : throw new ArgumentNullException(nameof(hexGrid));
        _gridRaycaster = gridRaycaster != null ? gridRaycaster : throw new ArgumentNullException(nameof(gridRaycaster));
        _isWorking = true;
    }
}

