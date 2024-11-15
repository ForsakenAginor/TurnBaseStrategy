using Assets.Scripts.HexGrid;
using System;
using UnityEngine;

public class CellSelector : MonoBehaviour
{
    private HexGridXZ<CellSprite> _hexGrid;
    private GridRaycaster _gridRaycaster;
    private bool _isWorking = false;

    public event Action<Vector3, Vector2Int> CellClicked;

    private void FixedUpdate()
    {
        if (_isWorking == false)
            return;

        if(Input.GetMouseButtonDown(0) && _gridRaycaster.TryGetPointerPosition(out Vector3 worldPosition))
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

