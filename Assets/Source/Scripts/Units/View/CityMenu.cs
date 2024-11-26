using System;
using UnityEngine;
using UnityEngine.UI;

public class CityMenu : MonoBehaviour, IUIElement
{
    private Button _upgradeButton;
    private Button _hireInfantry;
    private Button _hireSpearman;
    private Button _hireArcher;
    private Button _hireKnight;
    private IUIElement _buttonCanvas;

    private Action<UnitType, Vector3> TryHireCallback;
    private Func<Vector3, bool> TryUpgradeCityCallback;

    public void Enable()
    {
        _buttonCanvas.Enable();

        _upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        _hireInfantry.onClick.AddListener(OnHireInfantryClick);
        _hireArcher.onClick.AddListener(OnHireArcherClick);
        _hireSpearman.onClick.AddListener(OnHireSpearmanClick);
        _hireKnight.onClick.AddListener(OnHireKnightClick);
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

    public void Init(Action<UnitType, Vector3> tryHireCallback, Func<Vector3, bool> tryUpgradeCityCallback,
        Button upgradeButton, Button hireInfantry, Button hireSpearman,
        Button hireArcher, Button hireKnight, IUIElement buttonCanvas)
    {
        TryHireCallback = tryHireCallback != null ? tryHireCallback : throw new ArgumentNullException(nameof(tryHireCallback));
        TryUpgradeCityCallback = tryUpgradeCityCallback != null ? tryUpgradeCityCallback : throw new ArgumentNullException(nameof(tryUpgradeCityCallback));
        _hireInfantry = hireInfantry != null ? hireInfantry : throw new ArgumentNullException(nameof(hireInfantry));
        _hireSpearman = hireSpearman != null ? hireSpearman : throw new ArgumentNullException(nameof(hireSpearman));
        _hireArcher = hireArcher != null ? hireArcher : throw new ArgumentNullException(nameof(hireArcher));
        _hireKnight = hireKnight != null ? hireKnight : throw new ArgumentNullException(nameof(hireKnight));
        _upgradeButton = upgradeButton != null ? upgradeButton : throw new ArgumentNullException(nameof(upgradeButton));
        _buttonCanvas = buttonCanvas != null ? buttonCanvas : throw new ArgumentNullException(nameof(buttonCanvas));
    }

    private void OnHireKnightClick()
    {
        TryHireCallback.Invoke(UnitType.Knight, transform.position);
    }

    private void OnHireSpearmanClick()
    {
        Debug.Log("spearman hired");
        //_tryHireCallback.Invoke(UnitType.Spearman, transform.position);
    }

    private void OnHireArcherClick()
    {
        Debug.Log("archer hired");
        //_tryHireCallback.Invoke(UnitType.Archer, transform.position);
    }

    private void OnHireInfantryClick()
    {
        TryHireCallback.Invoke(UnitType.Infantry, transform.position);
    }

    private void OnUpgradeButtonClick()
    {
        bool isUpgraded = TryUpgradeCityCallback.Invoke(transform.position);
        Debug.Log($"City upgraded: {isUpgraded}");
    }
}