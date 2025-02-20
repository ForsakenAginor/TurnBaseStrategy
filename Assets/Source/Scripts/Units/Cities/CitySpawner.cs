using Assets.Scripts.HexGrid;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CitySpawner : MonoBehaviour, IUnitSpawner, IPlayerUnitSpawner
{
    private readonly Dictionary<CitySize, string> _fortificationSymbols = new Dictionary<CitySize, string>()
    {
        {CitySize.Village, "II" },
        {CitySize.Town, "III" },
        {CitySize.City, "IV" },
        {CitySize.Castle, "IV" },
    };
    private readonly Dictionary<IncomeGrade, string> _incomeSymbols = new Dictionary<IncomeGrade, string>()
    {
        {IncomeGrade.Tier1, "II" },
        {IncomeGrade.Tier2, "III" },
        {IncomeGrade.Tier3, "IV" },
        {IncomeGrade.Tier4, "IV" },
    };

    [SerializeField] private SwitchableElement _buttonCanvas;

    [Header("Upgrades with tiers")]
    [SerializeField] private TMP_Text _upgradeFortIcon;
    [SerializeField] private TMP_Text _upgradeFortCost;
    [SerializeField] private TMP_Text _upgradeIncomeIcon;
    [SerializeField] private TMP_Text _upgradeIncomeCost;

    [Header("Hire buttons")]
    [SerializeField] private HireButton _hireInfantry;
    [SerializeField] private HireButton _hireSpearman;
    [SerializeField] private HireButton _hireArcher;
    [SerializeField] private HireButton _hireKnight;
    [SerializeField] private HireButton _hireMage;

    [Header("Upgrade buttons")]
    [SerializeField] private HireButton _upgradeFortification;
    [SerializeField] private HireButton _upgradeIncome;
    [SerializeField] private HireButton _upgradeArchers;
    [SerializeField] private HireButton _upgradeSpearmen;
    [SerializeField] private HireButton _upgradeKnights;
    [SerializeField] private HireButton _upgradeMages;


    private CitiesConfiguration _configuration;
    private ICityUpgradesCostConfiguration _economyConfiguration;
    private CitiesFactory _factory;
    private CitiesActionsManager _unitsManager;
    private HexGridXZ<Unit> _grid;
    private UnitSpawner _unitSpawner;
    private Resource _firstWallet;
    private Resource _secondWallet;
    private Dictionary<Vector2Int, string> _citiesNames = new();
    private bool _isHotsit;

    public event Action<Unit, string> UnitSpawned;
    public event Action<UnitView> UnitViewSpawned;

    public Action<AudioSource> AudioSourceCallback;

    private void OnDestroy()
    {
        _unitsManager.CityCaptured -= OnCityCaptured;
    }

    public void Init(SerializedPair<Vector2Int, string>[] citiesNames, CitiesActionsManager manager, UnitSpawner unitSpawner, Resource wallet,
        CitiesConfiguration configuration, ICityUpgradesCostConfiguration economyConfiguration,
        HexGridXZ<Unit> grid, Action<AudioSource> callback)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _firstWallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _economyConfiguration = economyConfiguration != null ? economyConfiguration : throw new ArgumentNullException(nameof(economyConfiguration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        AudioSourceCallback = callback != null ? callback : throw new ArgumentNullException(nameof(callback));
        _factory = new CitiesFactory(configuration);

        if (citiesNames != null)
        {
            foreach (var item in citiesNames)
                _citiesNames.Add(item.Key, item.Value);
        }
        else
        {
            throw new ArgumentNullException(nameof(citiesNames));
        }

        _unitsManager.CityCaptured += OnCityCaptured;
    }

    public void InitHotSit(SerializedPair<Vector2Int, string>[] citiesNames, CitiesActionsManager manager, UnitSpawner unitSpawner,
        Resource firstWallet, Resource secondWallet,
        CitiesConfiguration configuration, ICityUpgradesCostConfiguration economyConfiguration,
        HexGridXZ<Unit> grid, Action<AudioSource> callback)
    {
        _unitsManager = manager != null ? manager : throw new ArgumentNullException(nameof(manager));
        _unitSpawner = unitSpawner != null ? unitSpawner : throw new ArgumentNullException(nameof(unitSpawner));
        _firstWallet = firstWallet != null ? firstWallet : throw new ArgumentNullException(nameof(firstWallet));
        _secondWallet = secondWallet != null ? secondWallet : throw new ArgumentNullException(nameof(secondWallet));
        _configuration = configuration != null ? configuration : throw new ArgumentNullException(nameof(configuration));
        _economyConfiguration = economyConfiguration != null ? economyConfiguration : throw new ArgumentNullException(nameof(economyConfiguration));
        _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
        AudioSourceCallback = callback != null ? callback : throw new ArgumentNullException(nameof(callback));
        _factory = new CitiesFactory(configuration);

        if (citiesNames != null)
        {
            foreach (var item in citiesNames)
                _citiesNames.Add(item.Key, item.Value);
        }
        else
        {
            throw new ArgumentNullException(nameof(citiesNames));
        }

        _unitsManager.CityCaptured += OnCityCaptured;
        _isHotsit = true;
    }

    public void SpawnCity(Vector2Int position, CitySize size, Side side, bool isVisible, CityUpgrades upgrades = null,
        bool mustCreateWithMaxHealth = true, int health = int.MinValue)
    {
        if (_grid.GetGridObject(position) != null)
            throw new Exception("Can't create city: cell is not empty");

        var unit = _factory.Create(size, side, upgrades, mustCreateWithMaxHealth, health);
        CityFacade facadePrefab = null;

        switch(side)
        {
            case Side.Player:
                facadePrefab = _configuration.GetPlayerPrefab(size);
                break;
            case Side.Enemy:
                facadePrefab = _configuration.GetEnemyPrefab(size);
                break;
            case Side.Neutral:
                facadePrefab = _configuration.GetNeutralPrefab(size);
                break;
        }

        var facade = Instantiate(facadePrefab, _grid.GetCellWorldPosition(position), Quaternion.identity);

        ICityUpgrades upgrade = unit.Upgrades;
        
        facade.UnitView.Init(unit, AudioSourceCallback);
        facade.Menu.Init(TryHireUnit, TryUpgradeCity,
            _hireInfantry, _hireSpearman, _hireArcher, _hireKnight, _hireMage,
            _upgradeArchers, _upgradeSpearmen, _upgradeKnights, _upgradeMages, _upgradeIncome, _upgradeFortification,
            _buttonCanvas,
            _upgradeFortCost, _economyConfiguration.FortificationCost[size], _upgradeFortIcon, _fortificationSymbols[size],
            _upgradeIncomeCost, _economyConfiguration.IncomeCost[upgrade.Income], _upgradeIncomeIcon, _incomeSymbols[upgrade.Income],
            size, upgrade);

        facade.CityName.Init(_citiesNames[position]);
        _unitsManager.AddCity(unit, facade, isVisible);
        facade.UnitView.ShowTitle();

        UnitSpawned?.Invoke(unit, _citiesNames[position]);

        if (side == Side.Player)
            UnitViewSpawned?.Invoke(facade.UnitView);
    }

    private void OnCityCaptured(Vector2Int cell, Side side)
    {
        SpawnCity(cell, CitySize.Village, side, true);
    }

    private bool TryUpgradeCity(Upgrades upgradeType, Vector3 position)
    {
        var city = _grid.GetGridObject(position) as CityUnit;
        Side side = city.Side;
        CitySize size = city.CitySize;
        Vector2Int cell = _grid.GetXZ(position);
        int cost;

        switch (upgradeType)
        {
            case Upgrades.Archers:
                cost = _economyConfiguration.ArcherCost;
                break;

            case Upgrades.Spearmen:
                cost = _economyConfiguration.SpearmanCost;
                break;

            case Upgrades.Knights:
                cost = _economyConfiguration.KnightCost;
                break;

            case Upgrades.Mages:
                cost = _economyConfiguration.MageCost;
                break;

            case Upgrades.Income:
                cost = _economyConfiguration.IncomeCost[city.Upgrades.Income];
                break;

            case Upgrades.Fortifications:
                cost = _economyConfiguration.FortificationCost[size];
                break;

            default:
                throw new Exception("Upgrade type not representing in CitySpawner switch module");
        }

        if (_isHotsit)
        {
            if (side == Side.Player)
            {
                if (_firstWallet.TrySpent(cost) == false)
                    return false;
            }
            else if (side == Side.Enemy)
            {
                if (_secondWallet.TrySpent(cost) == false)
                    return false;
            }
        }
        else
        {
            if (side == Side.Player)
                if (_firstWallet.TrySpent(cost) == false)
                    return false;
        }

        if (upgradeType == Upgrades.Fortifications)
        {
            CityUpgrades upgrades = _unitsManager.RemoveCity(city);
            size++;
            SpawnCity(cell, size, side, true, upgrades);
            _buttonCanvas.Disable();
            return true;
        }
        else
        {
            CityUpgrades upgrades = _unitsManager.RemoveCity(city);
            upgrades.Upgrade(upgradeType);
            int health = city.Health;
            SpawnCity(cell, size, side, true, upgrades, false, health);
            _buttonCanvas.Disable();
            return true;
        }
    }

    private bool TryHireUnit(UnitType type, Vector3 position)
    {
        var city = _grid.GetGridObject(position);
        Vector2Int cell = _grid.GetXZ(position);
        return _unitSpawner.TrySpawnUnit(cell, type, city.Side);
    }
}
