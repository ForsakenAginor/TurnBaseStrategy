using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Attacker))]
public class WalkableUnitFacade : UnitFacade, IWalkableUnitFacade
{
    private Mover _mover;
    private Attacker _attacker;

    public Mover Mover => _mover;

    public Attacker Attacker => _attacker;

    protected override void InitializeFieldsInAwake()
    {
        _mover = GetComponent<Mover>();
        _attacker = GetComponent<Attacker>();
    }
}