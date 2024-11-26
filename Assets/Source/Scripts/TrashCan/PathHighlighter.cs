using Assets.Scripts.HexGrid;
using HexPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathHighlighter : MonoBehaviour, IControllable
{
    [SerializeField] private CellSprite _unitColor = CellSprite.ContestedYellow;
    [SerializeField] private CellSprite _availableColor = CellSprite.ContestedYellow;
    [SerializeField] private CellSprite _notAvailableColor = CellSprite.ContestedYellow;
    [SerializeField] private CellSprite _blockedColor = CellSprite.ContestedYellow;

    private HexGridXZ<CellSprite> _hexGrid;
    private HexPathFinder _pathFinder;
    private CellSelector _selector;
    private List<Vector2Int> _path;
    private Vector2Int _rootCell;
    private bool _isWorking;

    public void Init(HexGridXZ<CellSprite> hexGrid, CellSelector selector, HexPathFinder pathFinder)
    {
        _hexGrid = hexGrid != null ? hexGrid : throw new ArgumentNullException(nameof(hexGrid));
        _selector = selector != null ? selector : throw new ArgumentNullException(nameof(selector));
        _pathFinder = pathFinder != null ? pathFinder : throw new ArgumentNullException(nameof(pathFinder));
    }

    public void EnableControl()
    {
        if (_isWorking)
            return;

        _isWorking = true;
        _selector.CellClicked += OnCellClicked;
    }

    public void DisableControl()
    {
        if (_isWorking == false)
            return;

        _selector.CellClicked -= OnCellClicked;
        _hexGrid.SetGridObject(_rootCell.x, _rootCell.y, CellSprite.Empty);
        _isWorking = false;
    }

    public void SetRootCell(Vector2Int position)
    {
        _rootCell = position;
        _hexGrid.SetGridObject(position.x, position.y, _unitColor);
    }

    private void OnCellClicked(Vector3 _, Vector2Int position)
    {
        if (position == _rootCell)
            return;

        if(_path != null)
            foreach (var item in _path)
                _hexGrid.SetGridObject(item.x, item.y, CellSprite.Empty);        

        _path = _pathFinder.FindPath(_rootCell, position);

        if(_path == null)
            return;

        _path = _path.Skip(1).ToList();

        for (int i = 0; i < _path.Count; i++)
            _hexGrid.SetGridObject(_path[i].x, _path[i].y, _availableColor);
    }
}