using System;
using UnityEngine;

public class PlayerMovementManager
{
    private readonly CellSelector _cellSelector;
    private readonly Mover _mover;

    public PlayerMovementManager(CellSelector cellSelector, Mover mover)
    {
        _cellSelector = cellSelector != null ? cellSelector : throw new ArgumentNullException(nameof(cellSelector));
        _mover = mover != null ? mover : throw new ArgumentNullException(nameof(_mover));

        _cellSelector.CellClicked += OnCellClicked;
    }

    ~PlayerMovementManager()
    {
        _cellSelector.CellClicked -= OnCellClicked;
    }

    private void OnCellClicked(Vector3 position)
    {
        _mover.Move(position);
    }
}
