using System;
using System.Collections.Generic;
using Assets.Scripts.HexGrid;
using UnityEngine;

public class CellHighlighter
{
    private readonly CellSprite _unitColor;
    private readonly CellSprite _selectedUnitColor;
    private readonly CellSprite _availableColor;
    private readonly CellSprite _notAvailableColor;
    private readonly CellSprite _enemyColor;

    private readonly NewInputSorter _inputSorter;
    private readonly HexGridXZ<CellSprite> _hexGrid;
    private readonly List<Vector2Int> _coloredCells = new List<Vector2Int>();

    public CellHighlighter(NewInputSorter inputSorter, HexGridXZ<CellSprite> grid, GridColorConfiguration configuration)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new ArgumentNullException(nameof(inputSorter));
        _hexGrid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        
        if(configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _unitColor = configuration.GetAllyCellColor();
        _selectedUnitColor = configuration.GetSelectedCellColor();
        _availableColor = configuration.GetAvailableCellColor();
        _notAvailableColor = configuration.GetBlockedCellColor();
        _enemyColor = configuration.GetEnemyCellColor();

        _inputSorter.MovableUnitSelected += OnMovableUnitSelected;
        _inputSorter.EnemySelected += OnEnemySelected;
        _inputSorter.FriendlyCitySelected += OnFriendlyCitySelected;
        _inputSorter.BecomeInactive += OnBecameInactive;
    }

    ~CellHighlighter()
    {
        _inputSorter.MovableUnitSelected -= OnMovableUnitSelected;
        _inputSorter.EnemySelected -= OnEnemySelected;
        _inputSorter.FriendlyCitySelected -= OnFriendlyCitySelected;
        _inputSorter.BecomeInactive -= OnBecameInactive;
    }

    private void OnFriendlyCitySelected(Vector2Int position)
    {
        ClearGrid();
        ColorizeSelectedCell(position, _selectedUnitColor);
    }

    private void OnEnemySelected(Vector2Int position)
    {
        ClearGrid();
        ColorizeSelectedCell(position, _enemyColor);
    }

    private void OnMovableUnitSelected(Vector2Int selectedUnit,
        IEnumerable<Vector2Int> possibleWays, IEnumerable<Vector2Int> blockedCells,
        IEnumerable<Vector2Int> friendlyCells, IEnumerable<Vector2Int> possiblesAttacks)
    {
        ClearGrid();

        ColorizeSelectedCell(selectedUnit, _selectedUnitColor);

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