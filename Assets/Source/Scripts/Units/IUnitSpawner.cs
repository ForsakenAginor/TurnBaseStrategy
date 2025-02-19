using System;

public interface IUnitSpawner
{
    public event Action<Unit, string> UnitSpawned;
}

public interface IPlayerUnitSpawner
{
    public event Action<UnitView> UnitViewSpawned;
}