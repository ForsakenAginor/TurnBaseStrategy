public class CityUnit : Unit
{
    private CitySize _citySize;

    public CityUnit(CitySize citySize,
        Side side, Resource health, int counterAttackPower) : base(side, health, counterAttackPower)
    {
        _citySize = citySize;
    }

    public CitySize CitySize => _citySize;

    public void DestroyCity() => Kill();
}