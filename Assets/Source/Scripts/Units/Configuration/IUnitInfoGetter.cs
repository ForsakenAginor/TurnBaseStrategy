public interface IUnitInfoGetter
{
    public (int attack, int counterAttack, int health, int steps) GetUnitInfo(UnitType type); 
}
