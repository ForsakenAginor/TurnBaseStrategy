using System;
using System.Collections.Generic;
using System.Linq;

public class TaxSystem : IResetable, IIncome
{
    private readonly Resource _wallet;
    private readonly IUnitSpawner _citySpawner;
    private readonly IUnitSpawner _unitSpawner;
    private readonly Dictionary<Unit, int> _units = new();
    private readonly Dictionary<Unit, int> _cities = new();
    private readonly ICityEconomicInfoGetter _cityEconomicConfiguration;
    private readonly IUnitCostGetter _unitConfiguration;
    private readonly Side _side;

    public TaxSystem(Resource wallet, IUnitSpawner citySpawner, IUnitSpawner unitSpawner,
        ICityEconomicInfoGetter cityConfiguration, IUnitCostGetter unitConfiguration, Side side)
    {
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _citySpawner = citySpawner != null ? citySpawner : throw new ArgumentNullException(nameof(citySpawner));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _cityEconomicConfiguration = cityConfiguration != null ? cityConfiguration : throw new ArgumentNullException(nameof(cityConfiguration));
        _unitConfiguration = unitConfiguration != null ? unitConfiguration : throw new ArgumentNullException(nameof(unitConfiguration));
        _side = side;

        _citySpawner.UnitSpawned += OnCitySpawned;
        _unitSpawner.UnitSpawned += OnUnitSpawned;
    }

    ~TaxSystem()
    {
        _citySpawner.UnitSpawned -= OnCitySpawned;
        _unitSpawner.UnitSpawned -= OnUnitSpawned;
    }

    public event Action<int> IncomeChanged;
    public event Action<List<KeyValuePair<string, int>>> IncomeCompositionChanged;
    public event Action CloseToBankrupt;
    public event Action FarToBankrupt;

    public void Reset()
    {
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
        if (unit.Side != _side)
            return;

        unit.Destroyed += OnUnitDied;
        var walkableUnit = unit as WalkableUnit;
        _units.Add(unit, _unitConfiguration.GetUnitSalary(walkableUnit.UnitType));

        CalcIncome();
    }

    private void OnCitySpawned(Unit unit)
    {
        if (unit.Side != _side)
            return;

        unit.Destroyed += OnCityDestroyed;
        var city = unit as CityUnit;
        _cities.Add(unit, _cityEconomicConfiguration.GetGoldIncome(city.CitySize));

        CalcIncome();
    }

    private void OnUnitDied(Unit unit)
    {
        if (_units.ContainsKey(unit))
            _units.Remove(unit);
        else
            throw new InvalidOperationException("Diying unit not representing in TaxSystem unit list");

        unit.Destroyed -= OnUnitDied;

        CalcIncome();
    }

    private void OnCityDestroyed(Unit unit)
    {
        if (_cities.ContainsKey(unit))
            _cities.Remove(unit);
        else
            throw new InvalidOperationException("Destroying city not representing in TaxSystem cities list");

        unit.Destroyed -= OnCityDestroyed;

        CalcIncome();
    }

    private void CalcIncome()
    {
        int income = 0;
        List<KeyValuePair<string, int>> incomeParts = new();

        foreach (var city in _cities.Keys)
        {
            income += _cities[city];
            var cityUnit = city as CityUnit;
            incomeParts.Add(new KeyValuePair<string, int>(cityUnit.CitySize.ToString(), _cities[city]));
        }

        foreach (var unit in _units.Keys)
        {
            income -= _units[unit];
            var walkableUnit = unit as WalkableUnit;
            incomeParts.Add(new KeyValuePair<string, int>(walkableUnit.UnitType.ToString(), -_units[unit]));
        }

        int nextValue = _wallet.Amount + income;

        if (nextValue < 0)
            CloseToBankrupt?.Invoke();
        else
            FarToBankrupt?.Invoke();

        IncomeChanged?.Invoke(income);
        IncomeCompositionChanged?.Invoke(incomeParts);
    }
}

public class AITaxSystem : IResetable
{
    private readonly IUnitSpawner _citySpawner;
    private readonly IUnitSpawner _unitSpawner;
    private readonly List<Unit> _unitsAI = new();
    private readonly List<Unit> _citiesAI = new();
    private readonly Side _side;

    public AITaxSystem(IUnitSpawner citySpawner, IUnitSpawner unitSpawner, Side side)
    {
        _citySpawner = citySpawner != null ? citySpawner : throw new ArgumentNullException(nameof(citySpawner));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _side = side;
        _citySpawner.UnitSpawned += OnCitySpawned;
        _unitSpawner.UnitSpawned += OnUnitSpawned;
    }

    ~AITaxSystem()
    {
        _citySpawner.UnitSpawned -= OnCitySpawned;
        _unitSpawner.UnitSpawned -= OnUnitSpawned;
    }

    public void Reset()
    {
        foreach (var city in _citiesAI)
            city.HealingUnit();

        foreach (var unit in _unitsAI)
            unit.HealingUnit();
    }

    private void OnUnitDied(Unit unit)
    {
        if (_unitsAI.Contains(unit))
            _unitsAI.Remove(unit);
        else
            throw new InvalidOperationException("Diying unit not representing in TaxSystem unit list");

        unit.Destroyed -= OnUnitDied;
    }

    private void OnCityDestroyed(Unit unit)
    {
        if (_citiesAI.Contains(unit))
            _citiesAI.Remove(unit);
        else
            throw new InvalidOperationException("Destroying city not representing in TaxSystem cities list");

        unit.Destroyed -= OnCityDestroyed;
    }

    private void OnUnitSpawned(Unit unit)
    {
        if (unit.Side != _side)
            return;

        unit.Destroyed += OnUnitDied;
        var walkableUnit = unit as WalkableUnit;
        _unitsAI.Add(walkableUnit);
    }

    private void OnCitySpawned(Unit unit)
    {
        if (unit.Side != _side)
            return;

        unit.Destroyed += OnCityDestroyed;
        var city = unit as CityUnit;
        _citiesAI.Add(city);
    }
}
