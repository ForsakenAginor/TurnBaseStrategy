using Lean.Localization;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SwitchableElement _tooltip;
    [SerializeField] private TMP_Text _tooltipTMP;
    [SerializeField] private string _leanPhrase;
    [SerializeField] private Button _button;

    private Vector3 _offset = new Vector3(100, -100, 0);

    private void Start()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        _tooltip.Disable();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.Disable();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltip.transform.localPosition = transform.localPosition + _offset;
        string text = LeanLocalization.GetTranslationText(_leanPhrase);
        _tooltipTMP.text = text;
        _tooltip.Enable();
    }
}
