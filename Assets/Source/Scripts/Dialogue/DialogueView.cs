using DG.Tweening;
using Lean.Localization;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _duration;
    [SerializeField] private Image _portrait;
    [SerializeField] private SwitchableElement _holder;
    [SerializeField] private Button _closeButton;

    private Dialogue _dialogue;

    private void OnDestroy()
    {
        _dialogue.BossSpawned -= OnBossSpawned;
        _closeButton.onClick.RemoveListener(OnCloseButtonClick);
    }

    public void Init(Dialogue dialogue)
    {
        _dialogue = dialogue != null ? dialogue : throw new ArgumentNullException(nameof(dialogue));
        _dialogue.BossSpawned += OnBossSpawned;
        _closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnCloseButtonClick()
    {
        _holder.Disable();
    }

    private void OnBossSpawned(Sprite sprite, string text)
    {
        _holder.Enable();
        _portrait.sprite = sprite;
        _text.text = string.Empty;
        string bossPhrase = LeanLocalization.GetTranslationText(text);
        _text.DOText(bossPhrase, _duration, false);
    }
}