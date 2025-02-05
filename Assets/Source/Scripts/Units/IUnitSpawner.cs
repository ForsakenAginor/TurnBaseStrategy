using System;

public interface IUnitSpawner
{
    public event Action<Unit> UnitSpawned;
}

public interface IPlayerUnitSpawner
{
    public event Action<UnitView> UnitViewSpawned;
}