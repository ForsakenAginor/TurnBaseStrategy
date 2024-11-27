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

    public void Init(IResource wallet)
    {
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _text.text = _wallet.Amount.ToString();
        _wallet.ResourcesAmountChanged += OnWalletChanged;
    }

    private void OnWalletChanged()
    {
        _isNeedToUpdate = true;
    }
}
