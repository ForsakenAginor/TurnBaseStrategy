public interface IWalkableUnitFacade : IUnitFacade
{
    public Mover Mover { get; }

    public Attacker Attacker { get; }
}
