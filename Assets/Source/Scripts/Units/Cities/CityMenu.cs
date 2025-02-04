using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityMenu : MonoBehaviour, ISwitchableElement
{
    private ISwitchableElement _upgradePanel;
    private Button _upgradeButton;
    private HireButton _hireInfantry;
    private HireButton _hireSpearman;
    private HireButton _hireArcher;
    private HireButton _hireKnight;
    private ISwitchableElement _buttonCanvas;
    private TMP_Text _upgradeCostTextField;
    private int _upgradeCost;
    private string _symbol;
    private TMP_Text _symbolField;
    private CitySize _size;

    private Func<UnitType, Vector3, bool> TryHireCallback;
    private Func<Vector3, bool> TryUpgradeCityCallback;

    public void Enable()
    {
        _buttonCanvas.Enable();
        _upgradePanel.Enable();

        if (_upgradeButton.gameObject.activeSelf == false)
            _upgradeButton.gameObject.SetActive(true);

        _upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        _hireInfantry.HireUnitButton.onClick.AddListener(OnHireInfantryClick);
        _hireArcher.HireUnitButton.onClick.AddListener(OnHireArcherClick);
        _hireSpearman.HireUnitButton.onClick.AddListener(OnHireSpearmanClick);
        _hireKnight.HireUnitButton.onClick.AddListener(OnHireKnightClick);

        switch(_size)
        {
            case CitySize.Village:
                _hireInfantry.Activate();
                _hireSpearman.DeActivate();
                _hireArcher.DeActivate();
                _hireKnight.DeActivate();
                break;
            case CitySize.Town:
                _hireInfantry.Activate();
                _hireSpearman.Activate();
                _hireArcher.DeActivate();
                _hireKnight.DeActivate();
                break;
            case CitySize.City:
                _hireInfantry.Activate();
                _hireSpearman.Activate();
                _hireArcher.Activate();
                _hireKnight.DeActivate();
                break;
            case CitySize.Castle:
                _hireInfantry.Activate();
                _hireSpearman.Activate();
                _hireArcher.Activate();
                _hireKnight.Activate();
                break;
            default:
                break;
        }

        if (_upgradeCost > 0)
        {
            _upgradeCostTextField.text = _upgradeCost.ToString();
            _symbolField.text = _symbol;
            return;
        }

        _upgradeCostTextField.text = string.Empty;
        _upgradeButton.gameObject.SetActive(false);
        _upgradePanel.Disable();

    }

    public void Disable()
    {
        _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
        _hireInfantry.HireUnitButton.onClick.RemoveListener(OnHireInfantryClick);
        _hireArcher.HireUnitButton.onClick.RemoveListener(OnHireArcherClick);
        _hireSpearman.HireUnitButton.onClick.RemoveListener(OnHireSpearmanClick);
        _hireKnight.HireUnitButton.onClick.RemoveListener(OnHireKnightClick);

        _buttonCanvas.Disable();
    }

    private void OnDestroy()
    {
        Disable();
    }

    public void Init(Func<UnitType, Vector3, bool> tryHireCallback, Func<Vector3, bool> tryUpgradeCityCallback,
        Button upgradeButton, HireButton hireInfantry, HireButton hireSpearman,
        HireButton hireArcher, HireButton hireKnight, ISwitchableElement buttonCanvas,
        TMP_Text upgradeCostTextLabel, int upgradeCost, ISwitchableElement upgradePanel,
        TMP_Text symbolField, string symbol, CitySize size)
    {
        TryHireCallback = tryHireCallback != null ? tryHireCallback : throw new ArgumentNullException(nameof(tryHireCallback));
        TryUpgradeCityCallback = tryUpgradeCityCallback != null ? tryUpgradeCityCallback : throw new ArgumentNullException(nameof(tryUpgradeCityCallback));
        _hireInfantry = hireInfantry != null ? hireInfantry : throw new ArgumentNullException(nameof(hireInfantry));
        _hireSpearman = hireSpearman != null ? hireSpearman : throw new ArgumentNullException(nameof(hireSpearman));
        _hireArcher = hireArcher != null ? hireArcher : throw new ArgumentNullException(nameof(hireArcher));
        _hireKnight = hireKnight != null ? hireKnight : throw new ArgumentNullException(nameof(hireKnight));
        _upgradeButton = upgradeButton != null ? upgradeButton : throw new ArgumentNullException(nameof(upgradeButton));
        _buttonCanvas = buttonCanvas != null ? buttonCanvas : throw new ArgumentNullException(nameof(buttonCanvas));
        _upgradePanel = upgradePanel != null ? upgradePanel : throw new ArgumentNullException(nameof(upgradePanel));
        _symbolField = symbolField != null ? symbolField : throw new ArgumentNullException(nameof(symbolField));
        _symbol = symbol != null ? symbol : throw new ArgumentNullException(nameof(symbol));

        _upgradeCostTextField = upgradeCostTextLabel != null ? upgradeCostTextLabel : throw new ArgumentNullException(nameof(upgradeCostTextLabel));
        _upgradeCost = upgradeCost > 0 ? upgradeCost : 0;
        _size = size;
    }

    private void OnHireKnightClick()
    {
        TryHireCallback.Invoke(UnitType.Knight, transform.position);
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

    private void OnUpgradeButtonClick()
    {
        TryUpgradeCityCallback.Invoke(transform.position);
    }
}