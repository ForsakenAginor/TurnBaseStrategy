using System;

public interface IUnitSpawner
{
    public event Action<Unit> UnitSpawned;
}