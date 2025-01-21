using System;
using System.Collections.Generic;
using System.Linq;

public class TaxSystem : IResetable, IIncome
{
    private readonly Resource _wallet;
    private readonly IUnitSpawner _citySpawner;
    private readonly IUnitSpawner _unitSpawner;
    private readonly Dictionary<Unit, int> _units = new();
    private readonly List<Unit> _enemyUnits = new();
    private readonly Dictionary<Unit, int> _cities = new();
    private readonly List<Unit> _enemyCities = new();
    private readonly ICityEconomicInfoGetter _cityEconomicConfiguration;
    private readonly IUnitCostGetter _unitConfiguration;

    public TaxSystem(Resource wallet, IUnitSpawner citySpawner, IUnitSpawner unitSpawner,
        ICityEconomicInfoGetter cityConfiguration, IUnitCostGetter unitConfiguration)
    {
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _citySpawner = citySpawner != null ? citySpawner : throw new ArgumentNullException(nameof(citySpawner));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _cityEconomicConfiguration = cityConfiguration != null ? cityConfiguration : throw new ArgumentNullException(nameof(cityConfiguration));
        _unitConfiguration = unitConfiguration != null ? unitConfiguration : throw new ArgumentNullException(nameof(unitConfiguration));

        _citySpawner.UnitSpawned += OnCitySpawned;
        _unitSpawner.UnitSpawned += OnUnitSpawned;
    }

    ~TaxSystem()
    {
        _citySpawner.UnitSpawned -= OnCitySpawned;
        _unitSpawner.UnitSpawned -= OnUnitSpawned;
    }

    public event Action<int> IncomeChanged;

    public void Reset()
    {
        //enemy logic
        foreach (var city in _enemyCities)
            city.HealingUnit();

        foreach (var unit in _enemyUnits)
            unit.HealingUnit();

        //player logic
        foreach (var city in _cities.Keys)
            _wallet.Add(_cities[city]);

        bool isBankrupt = false;

        foreach (var unit in _units.Keys)
        {
            if (_wallet.TrySpent(_units[unit]) == false)
            {
                _wallet.Spent(_units[unit]);
                isBankrupt = true;
            }
        }

        var units = _units.Keys.Concat(_cities.Keys).ToList();

        foreach (var unit in units)
        {
            if (isBankrupt)
                unit.SufferPaymentDamage();
            else
                unit.HealingUnit();
        }
    }

    private void OnUnitSpawned(Unit unit)
    {
        unit.Destroyed += OnUnitDied;
        var walkableUnit = unit as WalkableUnit;

        if (unit.Side == Side.Enemy)
            _enemyUnits.Add(walkableUnit);
        else
            _units.Add(unit, _unitConfiguration.GetUnitSalary(walkableUnit.UnitType));

        CalcIncome();
    }

    private void OnUnitDied(Unit unit)
    {
        if (_units.ContainsKey(unit))
            _units.Remove(unit);
        else
            _enemyUnits.Remove(unit);

        unit.Destroyed -= OnUnitDied;

        CalcIncome();
    }

    private void OnCityDestroyed(Unit unit)
    {
        if (_cities.ContainsKey(unit))
            _cities.Remove(unit);
        else
            _enemyCities.Remove(unit);

        unit.Destroyed -= OnCityDestroyed;

        CalcIncome();
    }

    private void OnCitySpawned(Unit unit)
    {
        unit.Destroyed += OnCityDestroyed;
        var city = unit as CityUnit;

        if (unit.Side == Side.Enemy)
            _enemyCities.Add(city);
        else
            _cities.Add(unit, _cityEconomicConfiguration.GetGoldIncome(city.CitySize));

        CalcIncome();
    }

    private void CalcIncome()
    {
         int income = 0;

        foreach (var city in _cities.Keys)
            income += _cities[city];

        foreach (var unit in _units.Keys)
            income -= _units[unit];        

        IncomeChanged?.Invoke(income);
    }
}

public interface IIncome
{
    public event Action<int> IncomeChanged;
}

