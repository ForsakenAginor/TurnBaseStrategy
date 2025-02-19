using System;
using TMPro;
using UnityEngine;

public class CityMenu : MonoBehaviour, ISwitchableElement
{
    //Hire
    private HireButton _hireInfantry;
    private HireButton _hireSpearman;
    private HireButton _hireArcher;
    private HireButton _hireKnight;
    private HireButton _hireMage;

    //Upgrades
    private HireButton _upgradeFortification;
    private HireButton _upgradeIncome;
    private HireButton _upgradeSpearmen;
    private HireButton _upgradeArchers;
    private HireButton _upgradeKnights;
    private HireButton _upgradeMages;

    //upgrade fortification
    private TMP_Text _upgradeFortCostTMP;
    private int _upgradeFortCost;
    private string _symbolFort;
    private TMP_Text _symbolFortTMP;

    //upgrade income
    private TMP_Text _upgradeIncomeCostTMP;
    private int _upgradeIncomeCost;
    private string _symbolIncome;
    private TMP_Text _symbolIncomeTMP;

    private ISwitchableElement _buttonCanvas;
    private CitySize _size;
    private ICityUpgrades _cityUpgrades;

    private Func<UnitType, Vector3, bool> TryHireCallback;
    private Func<Upgrades, Vector3, bool> TryUpgradeCityCallback;

    public void Enable()
    {
        _buttonCanvas.Enable();

        _hireInfantry.HireUnitButton?.onClick.AddListener(OnHireInfantryClick);
        _hireArcher.HireUnitButton?.onClick.AddListener(OnHireArcherClick);
        _hireSpearman.HireUnitButton?.onClick.AddListener(OnHireSpearmanClick);
        _hireKnight.HireUnitButton?.onClick.AddListener(OnHireKnightClick);
        _hireMage.HireUnitButton?.onClick.AddListener(OnHireMageClick);

        _upgradeFortification.HireUnitButton?.onClick.AddListener(OnUpgradeFortClick);
        _upgradeIncome.HireUnitButton?.onClick.AddListener(OnUpgradeIncomeClick);
        _upgradeSpearmen.HireUnitButton?.onClick.AddListener(OnUpgradeSpearmen);
        _upgradeArchers.HireUnitButton?.onClick.AddListener(OnUpgradeArchers);
        _upgradeKnights.HireUnitButton?.onClick.AddListener(OnUpgradeKnights);
        _upgradeMages.HireUnitButton?.onClick.AddListener(OnUpgradeMages);

        SetupButtons(_cityUpgrades.IsMageUpgrade, _hireMage, _upgradeMages);
        SetupButtons(_cityUpgrades.IsKnightUpgrade, _hireKnight, _upgradeKnights);
        SetupButtons(_cityUpgrades.IsSpearmanUpgrade, _hireSpearman, _upgradeSpearmen);
        SetupButtons(_cityUpgrades.IsArcherUpgrade, _hireArcher, _upgradeArchers);

        if(_size == CitySize.Castle)
        {
            _symbolFortTMP.text = _symbolFort;
            _upgradeFortification.DeActivate();
        }
        else
        {
            _symbolFortTMP.text = _symbolFort;
            _upgradeFortCostTMP.text = _upgradeFortCost.ToString();
            _upgradeFortification.Activate();
        }

        if (_cityUpgrades.Income == IncomeGrade.Tier4)
        {
            _symbolIncomeTMP.text = _symbolIncome;
            _upgradeIncome.DeActivate();
        }
        else
        {
            _symbolIncomeTMP.text = _symbolIncome;
            _upgradeIncomeCostTMP.text = _upgradeIncomeCost.ToString();
            _upgradeIncome.Activate();
        }
    }

    public void Disable()
    {
        _hireInfantry.HireUnitButton?.onClick.RemoveListener(OnHireInfantryClick);
        _hireArcher.HireUnitButton?.onClick.RemoveListener(OnHireArcherClick);
        _hireSpearman.HireUnitButton?.onClick.RemoveListener(OnHireSpearmanClick);
        _hireKnight.HireUnitButton?.onClick.RemoveListener(OnHireKnightClick);
        _hireMage.HireUnitButton?.onClick.RemoveListener(OnHireMageClick);

        _upgradeFortification.HireUnitButton?.onClick.RemoveListener(OnUpgradeFortClick);
        _upgradeIncome.HireUnitButton?.onClick.RemoveListener(OnUpgradeIncomeClick);
        _upgradeSpearmen.HireUnitButton?.onClick.RemoveListener(OnUpgradeSpearmen);
        _upgradeArchers.HireUnitButton?.onClick.RemoveListener(OnUpgradeArchers);
        _upgradeKnights.HireUnitButton?.onClick.RemoveListener(OnUpgradeKnights);
        _upgradeMages.HireUnitButton?.onClick.RemoveListener(OnUpgradeMages);

        _buttonCanvas.Disable();
    }


    private void OnDestroy()
    {
        Disable();
    }

    public void Init(Func<UnitType, Vector3, bool> tryHireCallback, Func<Upgrades, Vector3, bool> tryUpgradeCityCallback,
        HireButton hireInfantry, HireButton hireSpearman, HireButton hireArcher, HireButton hireKnight, HireButton hireMage,
        HireButton upgradeArchers, HireButton upgradeSpearmen, HireButton upgradeKnights, HireButton upgradeMages, HireButton upgradeIncome, HireButton upgradeFort,
        ISwitchableElement buttonCanvas,
        TMP_Text upgradeFortCostTMP, int upgradeFortCost, TMP_Text symbolFortTMP, string symbolFort,
        TMP_Text upgradeIncomeCostTMP, int upgradeIncomeCost, TMP_Text symbolIncomeTMP, string symbolIncome,
        CitySize size, ICityUpgrades cityUpgrades)
    {
        TryHireCallback = tryHireCallback != null ? tryHireCallback : throw new ArgumentNullException(nameof(tryHireCallback));
        TryUpgradeCityCallback = tryUpgradeCityCallback != null ? tryUpgradeCityCallback : throw new ArgumentNullException(nameof(tryUpgradeCityCallback));

        _hireInfantry = hireInfantry != null ? hireInfantry : throw new ArgumentNullException(nameof(hireInfantry));
        _hireSpearman = hireSpearman != null ? hireSpearman : throw new ArgumentNullException(nameof(hireSpearman));
        _hireArcher = hireArcher != null ? hireArcher : throw new ArgumentNullException(nameof(hireArcher));
        _hireKnight = hireKnight != null ? hireKnight : throw new ArgumentNullException(nameof(hireKnight));
        _hireMage = hireMage != null ? hireMage : throw new ArgumentNullException(nameof(hireMage));

        _upgradeArchers = upgradeArchers != null ? upgradeArchers : throw new ArgumentNullException(nameof(upgradeArchers));
        _upgradeSpearmen = upgradeSpearmen != null ? upgradeSpearmen : throw new ArgumentNullException(nameof(upgradeSpearmen));
        _upgradeKnights = upgradeKnights != null ? upgradeKnights : throw new ArgumentNullException(nameof(upgradeKnights));
        _upgradeMages = upgradeMages != null ? upgradeMages : throw new ArgumentNullException(nameof(upgradeMages));
        _upgradeIncome = upgradeIncome != null ? upgradeIncome : throw new ArgumentNullException(nameof(upgradeIncome));
        _upgradeFortification = upgradeFort != null ? upgradeFort : throw new ArgumentNullException(nameof(upgradeFort));

        _buttonCanvas = buttonCanvas != null ? buttonCanvas : throw new ArgumentNullException(nameof(buttonCanvas));

        _symbolFortTMP = symbolFortTMP != null ? symbolFortTMP : throw new ArgumentNullException(nameof(symbolFortTMP));
        _symbolFort = symbolFort != null ? symbolFort : throw new ArgumentNullException(nameof(symbolFort));
        _upgradeFortCostTMP = upgradeFortCostTMP != null ? upgradeFortCostTMP : throw new ArgumentNullException(nameof(upgradeFortCostTMP));
        _upgradeFortCost = upgradeFortCost > 0 ? upgradeFortCost : 0;

        _symbolIncomeTMP = symbolIncomeTMP != null ? symbolIncomeTMP : throw new ArgumentNullException(nameof(symbolIncomeTMP));
        _symbolIncome = symbolIncome != null ? symbolIncome : throw new ArgumentNullException(nameof(symbolIncome));
        _upgradeIncomeCostTMP = upgradeIncomeCostTMP != null ? upgradeIncomeCostTMP : throw new ArgumentNullException(nameof(upgradeIncomeCostTMP));
        _upgradeIncomeCost = upgradeIncomeCost > 0 ? upgradeIncomeCost : 0;

        _cityUpgrades = cityUpgrades != null ? cityUpgrades : throw new ArgumentNullException(nameof(cityUpgrades));
        _size = size;
    }

    private void SetupButtons(bool isUpgradeBought, HireButton hire, HireButton upgrade)
    {
        if (isUpgradeBought)
        {
            hire.Activate();
            upgrade.DeActivate();
        }
        else
        {
            hire.DeActivate();
            upgrade.Activate();
        }
    }

    private void OnUpgradeFortClick()
    {
        TryUpgradeCityCallback?.Invoke(Upgrades.Fortifications, transform.position);
    }

    private void OnUpgradeIncomeClick()
    {
        TryUpgradeCityCallback?.Invoke(Upgrades.Income, transform.position);
    }

    private void OnUpgradeSpearmen()
    {
        TryUpgradeCityCallback?.Invoke(Upgrades.Spearmen, transform.position);
    }

    private void OnUpgradeArchers()
    {
        TryUpgradeCityCallback?.Invoke(Upgrades.Archers, transform.position);
    }

    private void OnUpgradeKnights()
    {
        TryUpgradeCityCallback?.Invoke(Upgrades.Knights, transform.position);
    }

    private void OnUpgradeMages()
    {
        TryUpgradeCityCallback?.Invoke(Upgrades.Mages, transform.position);
    }

    private void OnHireKnightClick()
    {
        TryHireCallback.Invoke(UnitType.Knight, transform.position);
    }

    private void OnHireMageClick()
    {
        TryHireCallback.Invoke(UnitType.Wizard, transform.position);
    }

    private void OnHireSpearmanClick()
    {
        TryHireCallback.Invoke(UnitType.Spearman, transform.position);
    }

    private void OnHireArcherClick()
    {
        TryHireCallback.Invoke(UnitType.Archer, transform.position);
    }

    private void OnHireInfantryClick()
    {
        TryHireCallback.Invoke(UnitType.Infantry, transform.position);
    }
}