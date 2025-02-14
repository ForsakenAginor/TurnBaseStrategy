using System;
using TMPro;
using UnityEngine;

public class WalletView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private IResource _wallet;
    private bool _isNeedToUpdate = false;

    private void Update()
    {
        if (_isNeedToUpdate == false)
            return;

        _isNeedToUpdate = false;
        _text.text = _wallet.Amount.ToString();
    }

    private void OnDestroy()
    {
        _wallet.ResourcesAmountChanged -= OnWalletChanged;
    }

    public void Init(IResource wallet)
    {
        if (_wallet != null)
            _wallet.ResourcesAmountChanged -= OnWalletChanged;

        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        OnWalletChanged();
        _wallet.ResourcesAmountChanged += OnWalletChanged;
    }

    private void OnWalletChanged()
    {
        _isNeedToUpdate = true;
    }
}

public class EconomyFacade : IControllable
{
    private readonly WalletView _walletView;
    private readonly IncomeView _incomeView;
    private readonly IncomeCompositionView _incomeCompositionView;
    private readonly BunkruptView _bankruptView;
    private readonly IResource _wallet;
    private readonly TaxSystem _taxSystem;

    public EconomyFacade(WalletView walletView, IncomeView incomeView,
        IncomeCompositionView incomeCompositionView, BunkruptView bankruptView,
        IResource wallet, TaxSystem taxSystem)
    {
        _walletView = walletView != null ? walletView : throw new ArgumentNullException(nameof(walletView));
        _incomeView = incomeView != null ? incomeView : throw new ArgumentNullException(nameof(incomeView));
        _incomeCompositionView = incomeCompositionView != null ? incomeCompositionView : throw new ArgumentNullException(nameof(incomeCompositionView));
        _bankruptView = bankruptView != null ? bankruptView : throw new ArgumentNullException(nameof(bankruptView));
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _taxSystem = taxSystem != null ? taxSystem : throw new ArgumentNullException(nameof(taxSystem));
    }

    public void DisableControl()
    {
    }

    public void EnableControl()
    {
        _walletView.Init(_wallet);
        _incomeView.Init(_taxSystem);
        _incomeCompositionView.Init(_taxSystem);
        _bankruptView.Init(_taxSystem);

        _taxSystem.RefreshIncomeData();
    }
}
