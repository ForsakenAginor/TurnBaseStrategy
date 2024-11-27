using System;
using System.Collections.Generic;
using System.Linq;

public class TaxSystem : IResetable
{
    private readonly Resource _wallet;
    private readonly IUnitSpawner _citySpawner;
    private readonly IUnitSpawner _unitSpawner;
    private readonly Dictionary<Unit, int> _units = new ();
    private readonly Dictionary<Unit, int> _cities = new ();
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

    public void Reset()
    {
        foreach(var city in _cities.Keys)
            _wallet.Add(_cities[city]);        

        bool isBankrupt = false;

        foreach(var unit in _units.Keys)
        {
            if (_wallet.TrySpent(_units[unit]) == false)
            {
                _wallet.Spent(_units[unit]);
                isBankrupt = true;
            }
        }

        if (isBankrupt == false)
            return;

        var units = _units.Keys.Concat(_cities.Keys).ToList();

        foreach (var unit in units)
            unit.SufferPaymentDamage();
    }

    private void OnUnitSpawned(Unit unit)
    {
        if (unit.Side == Side.Enemy)
            return;

        unit.Destroyed += OnUnitDied;
        var walkableUnit = unit as WalkableUnit;
        _units.Add(unit, _unitConfiguration.GetUnitSalary(walkableUnit.UnitType));
    }

    private void OnUnitDied(Unit unit)
    {
        _units.Remove(unit);
        unit.Destroyed -= OnUnitDied;
    }

    private void OnCityDestroyed(Unit unit)
    {
        _cities.Remove(unit);
        unit.Destroyed -= OnCityDestroyed;
    }

    private void OnCitySpawned(Unit unit)
    {
        if (unit.Side == Side.Enemy)
            return;

        unit.Destroyed += OnCityDestroyed;
        var city = unit as CityUnit;
        _cities.Add(unit, _cityEconomicConfiguration.GetGoldIncome(city.CitySize));
    }
}
