using Assets.Scripts.HexGrid;
using Assets.Source.Scripts.GameLoop.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Root : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private UnitsConfiguration _unitConfiguration;
    [SerializeField] private CitiesConfiguration _cityConfiguration;

    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;
    [SerializeField] private CellHighlighter _cellHighlighter;
    [SerializeField] private GridRaycaster _gridRaycaster;
    [SerializeField] private CellSelector _cellSelector;

    [Header("Game progress")]
    [SerializeField] private GameStateMachineCreator _gameStateMachineCreator;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private CitySpawner _citySpawner;

    [Header("Gold")]
    [SerializeField] private WalletView _walletView;

    [Header("Debug")]
    [SerializeField] private Button _testButton;

    private void Start()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        _cellSelector.Init(_gridCreator.HexGrid, _gridRaycaster);
        var unitsGrid = _gridCreator.UnitsGrid;
        NewInputSorter inputSorter = new NewInputSorter(unitsGrid, _cellSelector, _gridCreator.HexGridView);
        _cellHighlighter = new (inputSorter, _gridCreator.HexGrid);

        //******** Wallet ***********
        Resource wallet = new Resource(0, int.MaxValue);
        TaxSystem taxSystem = new TaxSystem(wallet, _citySpawner, _unitSpawner, _cityConfiguration, _unitConfiguration);
        _walletView.Init(wallet);
        //********  Unit creation  ***********
        UnitsActionsManager unitManager = new UnitsActionsManager(inputSorter, unitsGrid);
        _unitSpawner.Init(unitManager, _unitConfiguration, unitsGrid, _gridCreator.HexGridView);
        CitiesActionsManager cityManager = new CitiesActionsManager(inputSorter, unitsGrid);
        _citySpawner.Init(cityManager, _unitSpawner, _cityConfiguration, unitsGrid);

        _citySpawner.SpawnCity(new Vector2Int(0, 0), CitySize.Village, Side.Player);
        _citySpawner.SpawnCity(new Vector2Int(5, 5), CitySize.Village, Side.Enemy);


        //********* Game state machine *******
        var resettables = unitManager.Units.ToList();
        resettables.Add(taxSystem);
        var stateMachine = _gameStateMachineCreator.Create(resettables, new List<IControllable>() { inputSorter });

        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();

        //********  Debug  ***********
        _testButton.onClick.AddListener(OnTestButtonClick);
    }

    private void OnTestButtonClick()
    {
        _unitSpawner.TrySpawnUnit(new Vector2Int(0, 0), UnitType.Infantry, Side.Player);
    }
}

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

        unit.Died += OnUnitDied;
        var walkableUnit = unit as WalkableUnit;
        _units.Add(unit, _unitConfiguration.GetUnitSalary(walkableUnit.UnitType));
    }

    private void OnUnitDied(Unit unit)
    {
        _units.Remove(unit);
        unit.Died -= OnUnitDied;
    }

    private void OnCityDestroyed(Unit unit)
    {
        _cities.Remove(unit);
        unit.Died -= OnCityDestroyed;
    }

    private void OnCitySpawned(Unit unit)
    {
        if (unit.Side == Side.Enemy)
            return;

        unit.Died += OnCityDestroyed;
        var city = unit as CityUnit;
        _cities.Add(unit, _cityEconomicConfiguration.GetGoldIncome(city.CitySize));
    }
}
