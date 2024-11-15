using System;
using System.Collections.Generic;
using Assets.Scripts.HexGrid;
using UnityEngine;

public class CellHighlighter
{
    private readonly CellSprite _unitColor = CellSprite.ContestedBlue;
    private readonly CellSprite _availableColor = CellSprite.ContestedGreen;
    private readonly CellSprite _notAvailableColor = CellSprite.ContestedRed;
    private readonly CellSprite _blockedColor = CellSprite.ContestedYellow;
    private readonly CellSprite _highlightedColor = CellSprite.ContestedYellow;
    private readonly InputSorter _inputSorter;
    private readonly HexGridXZ<CellSprite> _hexGrid;
    private readonly List<Vector2Int> _coloredCells = new List<Vector2Int>();


    public CellHighlighter(InputSorter inputSorter, HexGridXZ<CellSprite> grid)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new ArgumentNullException(nameof(inputSorter));
        _hexGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));

        _inputSorter.SelectionChanged += OnSelectionChanged;
        _inputSorter.PathCreated += OnPathCreated;
        _inputSorter.RoutSubmited += OnRoutSubmited;
        _inputSorter.BecomeInactive += OnBecameInactive;
    }

    ~CellHighlighter()
    {
        _inputSorter.SelectionChanged -= OnSelectionChanged;
        _inputSorter.PathCreated -= OnPathCreated;
        _inputSorter.RoutSubmited -= OnRoutSubmited;
        _inputSorter.BecomeInactive -= OnBecameInactive;
    }

    private void OnBecameInactive()
    {
        ClearGrid();
    }

    private void OnRoutSubmited(Rout _)
    {
        ClearGrid();
    }

    private void OnPathCreated(Rout rout)
    {
        ClearGrid();

        for (int i = 0; i < rout.ClosePartOfPath.Count; i++)
        {
            _hexGrid.SetGridObject(rout.ClosePartOfPath[i].x, rout.ClosePartOfPath[i].y, _availableColor);
            _coloredCells.Add(rout.ClosePartOfPath[i]);
        }

        for (int i = 0; i < rout.FarawayPartOfPath.Count; i++)
        {
            _hexGrid.SetGridObject(rout.FarawayPartOfPath[i].x, rout.FarawayPartOfPath[i].y, _notAvailableColor);
            _coloredCells.Add(rout.FarawayPartOfPath[i]);
        }

        ColorizeSelectedCell(rout.SelectedCell);
    }

    private void OnSelectionChanged(Vector2Int position)
    {
        ClearGrid();
        ColorizeSelectedCell(position);
    }

    private void ColorizeSelectedCell(Vector2Int position)
    {
        _coloredCells.Add(position);
        _hexGrid.SetGridObject(position.x, position.y, _unitColor);
    }

    private void ClearGrid()
    {
        for (int i = 0; i < _coloredCells.Count; i++)
            _hexGrid.SetGridObject(_coloredCells[i].x, _coloredCells[i].y, CellSprite.Empty);

        _coloredCells.Clear();
    }
}