using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CitiesUpgradeCost")]
public class CityUpgradesCostConfiguration : SerializedScriptableObject, ICityUpgradesCostConfiguration
{
    [SerializeField] private int _archerCost;
    [SerializeField] private int _spearmanCost;
    [SerializeField] private int _knightCost;
    [SerializeField] private int _mageCost;
    [SerializeField] Dictionary<CitySize, int> _fortificationCost;
    [SerializeField] Dictionary<IncomeGrade, int> _incomeCost;
    [SerializeField] Dictionary<IncomeGrade, int> _incomeValue;

    public int ArcherCost => _archerCost; 

    public int SpearmanCost => _spearmanCost;

    public int KnightCost => _knightCost;

    public int MageCost => _mageCost;

    public IReadOnlyDictionary<CitySize, int> FortificationCost => _fortificationCost;

    public IReadOnlyDictionary<IncomeGrade, int> IncomeCost => _incomeCost;

    public IReadOnlyDictionary<IncomeGrade, int> IncomeValue => _incomeValue;
}

public interface ICityUpgradesCostConfiguration
{
    public int ArcherCost { get; }

    public int SpearmanCost { get; }

    public int KnightCost { get; }

    public int MageCost { get; }

    public IReadOnlyDictionary<CitySize, int> FortificationCost { get; }

    public IReadOnlyDictionary<IncomeGrade, int> IncomeCost { get; }

    public IReadOnlyDictionary<IncomeGrade, int> IncomeValue { get; }
}