using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using System.Linq;

public class UnitManager
{
    private readonly Dictionary<WalkableUnit, Mover> _units = new Dictionary<WalkableUnit, Mover>();
    private readonly InputSorter _inputSorter;
    private readonly HexGridXZ<WalkableUnit> _grid;

    public UnitManager(InputSorter inputSorter, HexGridXZ<WalkableUnit> grid)
    {
        _inputSorter = inputSorter != null ? inputSorter : throw new System.ArgumentNullException(nameof(inputSorter));
        _grid = grid != null ? grid : throw new System.ArgumentNullException(nameof(grid));

        _inputSorter.RoutSubmited += OnRoutSubmited;
    }

    ~UnitManager()
    {
        _inputSorter.RoutSubmited -= OnRoutSubmited;        
    }

    public IEnumerable<IResetable> Units => _units.Keys;

    public void AddUnit(WalkableUnit unit, Mover transform)
    {
        if(unit == null)
            throw new ArgumentNullException(nameof(unit));

        if(transform == null)
            throw new ArgumentNullException(nameof(transform));

        if (_units.ContainsKey(unit))
            throw new ArgumentException("Unit already added");

        _units.Add(unit, transform);
        _grid.SetGridObject(transform.transform.position, unit);
    }

    private void OnRoutSubmited(Rout rout)
    {
        var unit = _grid.GetGridObject(rout.SelectedCell);

        if (rout.ClosePartOfPath.Count > 0 && unit.TryMoving(rout.ClosePartOfPath.Count))
        {
            _units[unit].Move(rout.Path);
            _grid.SetGridObject(rout.SelectedCell.x, rout.SelectedCell.y, null);
            _grid.SetGridObject(rout.ClosePartOfPath.Last().x, rout.ClosePartOfPath.Last().y, unit);
        }
    }
}