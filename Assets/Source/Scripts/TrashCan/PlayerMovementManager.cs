using System;
using UnityEngine;

public class PlayerMovementManager
{
    private readonly CellSelector _cellSelector;
    private Mover _mover;

    public PlayerMovementManager(CellSelector cellSelector)
    {
        _cellSelector = cellSelector != null ? cellSelector : throw new ArgumentNullException(nameof(cellSelector));

        _cellSelector.CellClicked += OnCellClicked;
    }

    ~PlayerMovementManager()
    {
        _cellSelector.CellClicked -= OnCellClicked;
    }

    public void SetMover(Mover mover)
    {
        _mover = mover;
    }

    private void OnCellClicked(Vector3 position, Vector2Int _)
    {/*
        if (_mover != null)
            _mover.Move(position);*/
    }
}
