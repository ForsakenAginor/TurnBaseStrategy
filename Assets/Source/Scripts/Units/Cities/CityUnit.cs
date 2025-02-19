using System;

public class CityUnit : Unit
{
    private readonly CityUpgrades _upgrades;
    private readonly CitySize _citySize;

    public CityUnit(CitySize citySize,
        Side side, Resource health, int counterAttackPower, CityUpgrades upgrades) : base(side, health, counterAttackPower)
    {
        _citySize = citySize;
        _upgrades = upgrades != null ? upgrades : new CityUpgrades();
    }

    public CitySize CitySize => _citySize;

    public CityUpgrades Upgrades => _upgrades;

    public void DestroyCity() => Kill();
}

public class CityUpgrades : ICityUpgrades
{
    private bool _isArcherUpgrade;
    private bool _isSpearmanUpgrade;
    private bool _isKnightUpgrade;
    private bool _isMageUpgrade;
    private IncomeGrade _incomeGrade = IncomeGrade.Tier1;

    public CityUpgrades()
    {
        _isArcherUpgrade = false;
        _isSpearmanUpgrade = false;
        _isKnightUpgrade = false;
        _isMageUpgrade = false;
        _incomeGrade = IncomeGrade.Tier1;
    }

    public CityUpgrades(bool isArcherUpgrade, bool isSpearmanUpgrade, bool isKnightUpgrade, bool isMageUpgrade, IncomeGrade incomeGrade)
    {
        _isArcherUpgrade = isArcherUpgrade;
        _isSpearmanUpgrade = isSpearmanUpgrade;
        _isKnightUpgrade = isKnightUpgrade;
        _isMageUpgrade = isMageUpgrade;
        _incomeGrade = incomeGrade;
    }

    public bool IsArcherUpgrade => _isArcherUpgrade;

    public bool IsSpearmanUpgrade => _isSpearmanUpgrade;

    public bool IsKnightUpgrade => _isKnightUpgrade;

    public bool IsMageUpgrade => _isMageUpgrade;

    public IncomeGrade Income => _incomeGrade;

    public void Upgrade(Upgrades upgrade)
    {
        switch (upgrade)
        {
            case Upgrades.Archers:
                BuyArcherUpgrade();
                    break;
            case Upgrades.Spearmen:
                BuySpearmanUpgrade();
                break;
            case Upgrades.Knights:
                BuyKnightUpgrade();
                break;
            case Upgrades.Mages:
                BuyMageUpgrade();
                break;
            case Upgrades.Income:
                BuyIncomeUpgrade();
                break;
            default:
                throw new ArgumentException("Can't upgrade city");
        }
    }

    private void BuyArcherUpgrade()
    {
        if (IsArcherUpgrade)
            throw new InvalidOperationException("City already had that upgrade");

        _isArcherUpgrade = true;
    }

    private void BuySpearmanUpgrade()
    {
        if (IsSpearmanUpgrade)
            throw new InvalidOperationException("City already had that upgrade");
   
        _isSpearmanUpgrade = true;
    }

    private void BuyKnightUpgrade()
    {
        if (IsKnightUpgrade)
            throw new InvalidOperationException("City already had that upgrade");
    
        _isKnightUpgrade = true;
    }

    private void BuyMageUpgrade()
    {
        if (IsMageUpgrade)
            throw new InvalidOperationException("City already had that upgrade");
    
        _isMageUpgrade = true;
    }

    private void BuyIncomeUpgrade()
    {
        if (_incomeGrade == IncomeGrade.Tier4)
            throw new InvalidOperationException("City already had that upgrade");

        _incomeGrade = (IncomeGrade) ((int)_incomeGrade + 1);
    }
}

public enum IncomeGrade
{
    Tier1,
    Tier2,
    Tier3,
    Tier4,
}

public enum Upgrades
{
    Archers,
    Spearmen,
    Knights,
    Mages,
    Income,
    Fortifications,
}

public interface ICityUpgrades
{
    public bool IsArcherUpgrade { get; }

    public bool IsSpearmanUpgrade { get; }

    public bool IsKnightUpgrade { get; }

    public bool IsMageUpgrade { get; }

    public IncomeGrade Income { get; }
}