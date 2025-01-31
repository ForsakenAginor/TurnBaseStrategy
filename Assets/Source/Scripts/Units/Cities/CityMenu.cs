using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityMenu : MonoBehaviour, IUIElement
{
    private IUIElement _upgradePanel;
    private Button _upgradeButton;
    private Button _hireInfantry;
    private Button _hireSpearman;
    private Button _hireArcher;
    private Button _hireKnight;
    private IUIElement _buttonCanvas;
    private TMP_Text _upgradeCostTextField;
    private int _upgradeCost;
    private string _symbol;
    private TMP_Text _symbolField;

    private Func<UnitType, Vector3, bool> TryHireCallback;
    private Func<Vector3, bool> TryUpgradeCityCallback;

    public void Enable()
    {
        _buttonCanvas.Enable();
        _upgradePanel.Enable();

        if (_upgradeButton.gameObject.activeSelf == false)
            _upgradeButton.gameObject.SetActive(true);

        _upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        _hireInfantry.onClick.AddListener(OnHireInfantryClick);
        _hireArcher.onClick.AddListener(OnHireArcherClick);
        _hireSpearman.onClick.AddListener(OnHireSpearmanClick);
        _hireKnight.onClick.AddListener(OnHireKnightClick);

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
        _hireInfantry.onClick.RemoveListener(OnHireInfantryClick);
        _hireArcher.onClick.RemoveListener(OnHireArcherClick);
        _hireSpearman.onClick.RemoveListener(OnHireSpearmanClick);
        _hireKnight.onClick.RemoveListener(OnHireKnightClick);

        _buttonCanvas.Disable();
    }

    private void OnDestroy()
    {
        Disable();
    }

    public void Init(Func<UnitType, Vector3, bool> tryHireCallback, Func<Vector3, bool> tryUpgradeCityCallback,
        Button upgradeButton, Button hireInfantry, Button hireSpearman,
        Button hireArcher, Button hireKnight, IUIElement buttonCanvas,
        TMP_Text upgradeCostTextLabel, int upgradeCost, IUIElement upgradePanel,
        TMP_Text symbolField, string symbol)
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