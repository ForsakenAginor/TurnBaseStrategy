using System;
using TMPro;
using UnityEngine;

public class CityShopView : MonoBehaviour
{
    [Header("Units")]
    [SerializeField] private TMP_Text _knightCost;
    [SerializeField] private TMP_Text _archerCost;
    [SerializeField] private TMP_Text _spearmanCost;
    [SerializeField] private TMP_Text _infantryCost;
    [SerializeField] private TMP_Text _mageCost;

    [Header("Upgrades")]
    [SerializeField] private TMP_Text _knightUpgradeCost;
    [SerializeField] private TMP_Text _archerUpgradeCost;
    [SerializeField] private TMP_Text _spearmanUpgradeCost;
    [SerializeField] private TMP_Text _mageUpgradeCost;

    public void Init(IUnitCostGetter configuration, ICityUpgradesCostConfiguration economyConfiguration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        if (economyConfiguration == null)
            throw new ArgumentNullException(nameof(economyConfiguration));

        _knightCost.text = configuration.GetUnitCost(UnitType.Knight).ToString();
        _archerCost.text = configuration.GetUnitCost(UnitType.Archer).ToString();
        _spearmanCost.text = configuration.GetUnitCost(UnitType.Spearman).ToString();
        _infantryCost.text = configuration.GetUnitCost(UnitType.Infantry).ToString();
        _mageCost.text = configuration.GetUnitCost(UnitType.Wizard).ToString();

        _spearmanUpgradeCost.text = economyConfiguration.SpearmanCost.ToString();
        _archerUpgradeCost.text = economyConfiguration.ArcherCost.ToString();
        _knightUpgradeCost.text = economyConfiguration.KnightCost.ToString();
        _mageUpgradeCost.text = economyConfiguration.MageCost.ToString();
    }
}
