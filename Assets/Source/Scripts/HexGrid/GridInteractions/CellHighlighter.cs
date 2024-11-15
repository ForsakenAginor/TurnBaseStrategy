using Assets.Scripts.HexGrid;
using System;
using UnityEngine;

public class CellHighlighter : MonoBehaviour, IControllable
{
    [SerializeField] private CellSprite _highlightedColor = CellSprite.ContestedYellow;

    private HexGridXZ<CellSprite> _hexGrid;
    private GridRaycaster _gridRaycaster;
    private bool _isWorking = false;
    private Vector2Int _lastCell;

    private void FixedUpdate()
    {
        if (_isWorking == false)
            return;

        if (_gridRaycaster.TryGetPointerPosition(out Vector3 worldPosition))
        {
            var position = _hexGrid.GetXZ(worldPosition);

            if (_hexGrid.IsValidGridPosition(position))
                _hexGrid.SetGridObject(position.x, position.y, _highlightedColor);

            if (_lastCell != position)
            {
                _hexGrid.SetGridObject(_lastCell.x, _lastCell.y, CellSprite.Empty);
                _lastCell = position;
            }
        }
    }

    public void Init(HexGridXZ<CellSprite> hexGrid, GridRaycaster gridRaycaster)
    {
        _hexGrid = hexGrid != null ? hexGrid : throw new ArgumentNullException(nameof(hexGrid));
        _gridRaycaster = gridRaycaster != null ? gridRaycaster : throw new ArgumentNullException(nameof(gridRaycaster));
    }

    public void EnableControl()
    {
        _isWorking = true;
    }

    public void DisableControl()
    {
        _hexGrid.SetGridObject(_lastCell.x, _lastCell.y, CellSprite.Empty);
        _isWorking = false;
    }
}