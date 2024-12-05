using System;
using TMPro;
using UnityEngine;

public class CityShopView : MonoBehaviour
{
    [SerializeField] private TMP_Text _knightCost;
    [SerializeField] private TMP_Text _archerCost;
    [SerializeField] private TMP_Text _spearmanCost;
    [SerializeField] private TMP_Text _infantryCost;

    public void Init(IUnitCostGetter configuration)
    {
        if(configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _knightCost.text = configuration.GetUnitCost(UnitType.Knight).ToString();
        _archerCost.text = configuration.GetUnitCost(UnitType.Archer).ToString();
        _spearmanCost.text = configuration.GetUnitCost(UnitType.Spearman).ToString();
        _infantryCost.text = configuration.GetUnitCost(UnitType.Infantry).ToString();
    }
}
