using UnityEngine;

[RequireComponent(typeof(Mover))]
public class WalkableUnitFacade : UnitFacade, IWalkableUnitFacade
{
    private Mover _mover;

    public Mover Mover => _mover;

    protected override void InitializeFieldsInAwake()
    {
        _mover = GetComponent<Mover>();
    }
}

