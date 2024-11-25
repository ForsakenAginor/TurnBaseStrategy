using System;
using System.Collections.Generic;
using UnityEngine;

public class Rout
{
    private readonly List<Vector2Int> _closePartOfPath;
    private readonly List<Vector2Int> _farawayPartOfPath;
    private readonly Vector2Int _selectedCell;
    private readonly List<Vector3> _path;

    public Rout(List<Vector2Int> closePartOfPath, List<Vector2Int> farawayPartOfPath, Vector2Int selectedCell, List<Vector3> path)
    {
        _closePartOfPath = closePartOfPath != null ? closePartOfPath : throw new ArgumentNullException(nameof(closePartOfPath));
        _farawayPartOfPath = farawayPartOfPath != null ? farawayPartOfPath : throw new ArgumentNullException(nameof(farawayPartOfPath));
        _selectedCell = selectedCell;
        _path = path != null ? path : throw new ArgumentNullException(nameof(path));
    }

    public List<Vector2Int> ClosePartOfPath => _closePartOfPath;

    public List<Vector2Int> FarawayPartOfPath => _farawayPartOfPath;

    public Vector2Int SelectedCell => _selectedCell;

    public List<Vector3> Path => _path;
}
