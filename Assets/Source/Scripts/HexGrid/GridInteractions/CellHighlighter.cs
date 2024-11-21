using System;
using System.Collections.Generic;
using Assets.Scripts.HexGrid;
using UnityEngine;

public class CellHighlighter
{
    private readonly CellSprite _unitColor = CellSprite.ContestedBlue;
    private readonly CellSprite _availableColor = CellSprite.ContestedGreen;
    private readonly CellSprite _notAvailableColor = CellSprite.ContestedOrange;
    private readonly CellSprite _enemyColor = CellSprite.ContestedRed;

    private readonly InputSorter _inputSorter;
    private readonly HexGridXZ<CellSprite> _hexGrid;
    private readonly List<Vector2Int> _coloredCells = new List<Vector2Int>();

    public CellHighlighter(InputSorter inputSorter, TestInputSorter testInputSorter, HexGridXZ<CellSprite> grid)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new ArgumentNullException(nameof(inputSorter));
        _hexGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        /*
        _inputSorter.SelectionChanged += OnSelectionChanged;
        _inputSorter.PathCreated += OnPathCreated;
        _inputSorter.RoutSubmited += OnRoutSubmited;
        _inputSorter.BecomeInactive += OnBecameInactive;
        */
        testInputSorter.MovableUnitSelected += OnMovableUnitSelected;
        testInputSorter.EnemySelected += OnEnemySelected;
        testInputSorter.BecomeInactive += OnBecameInactive;
    }

    ~CellHighlighter()
    {
        _inputSorter.SelectionChanged -= OnSelectionChanged;
        _inputSorter.PathCreated -= OnPathCreated;
        _inputSorter.RoutSubmited -= OnRoutSubmited;
        _inputSorter.BecomeInactive -= OnBecameInactive;
    }

    private void OnEnemySelected(Vector2Int position)
    {
        ClearGrid();
        ColorizeSelectedCell(position, _enemyColor);
    }

    private void OnMovableUnitSelected(
        IEnumerable<Vector2Int> possibleWays, IEnumerable<Vector2Int> blockedCells,
        IEnumerable<Vector2Int> friendlyCells, IEnumerable<Vector2Int> possiblesAttacks)
    {
        ClearGrid();

        foreach (var cell in possibleWays)
            ColorizeSelectedCell(cell, _availableColor);

        foreach (var cell in blockedCells)
            ColorizeSelectedCell(cell, _notAvailableColor); 

        foreach (var cell in friendlyCells)
            ColorizeSelectedCell(cell, _unitColor);

        foreach (var cell in possiblesAttacks)
            ColorizeSelectedCell(cell, _enemyColor);
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

        ColorizeSelectedCell(rout.SelectedCell, _unitColor);
    }

    private void OnSelectionChanged(Vector2Int position, Side side)
    {
        ClearGrid();

        CellSprite color = side == Side.Player ? _unitColor : _enemyColor;

        ColorizeSelectedCell(position, color);
    }

    private void ColorizeSelectedCell(Vector2Int position, CellSprite color)
    {
        _coloredCells.Add(position);
        _hexGrid.SetGridObject(position.x, position.y, color);
    }

    private void ClearGrid()
    {
        for (int i = 0; i < _coloredCells.Count; i++)
            _hexGrid.SetGridObject(_coloredCells[i].x, _coloredCells[i].y, CellSprite.Empty);

        _coloredCells.Clear();
    }
}