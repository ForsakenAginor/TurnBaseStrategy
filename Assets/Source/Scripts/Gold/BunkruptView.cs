using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BunkruptView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _colorFirst;
    [SerializeField] private Color _colorSecond;
    [SerializeField] private float _frequency;

    private TaxSystem _taxSystem;
    private Tween _tween;

    private void OnDestroy()
    {
        _taxSystem.CloseToBankrupt -= OnCloseToBunkrupt;
        _taxSystem.FarToBankrupt -= OnFarToBankrupt;
    }

    public void Init(TaxSystem taxSystem)
    {
        _taxSystem = taxSystem != null ? taxSystem : throw new ArgumentNullException(nameof(taxSystem));
        _taxSystem.CloseToBankrupt += OnCloseToBunkrupt;
        _taxSystem.FarToBankrupt += OnFarToBankrupt;
    }

    private void OnFarToBankrupt()
    {
        _image.gameObject.SetActive(false);
    }

    private void OnCloseToBunkrupt()
    {
        _image.gameObject.SetActive(true);

        if (_tween != null)
            _tween.Kill();

        _image.color = _colorFirst;
        _tween = _image.DOColor(_colorSecond, _frequency).SetLoops(-1, LoopType.Yoyo);
    }
}