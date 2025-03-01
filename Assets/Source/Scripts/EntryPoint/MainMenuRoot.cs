using Assets.Scripts.General;
using Assets.Scripts.Sound.AudioMixer;
using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuRoot : MonoBehaviour
{
    [SerializeField] private SoundInitializer _soundInitializer;
    [SerializeField] private Button _confirmNewGameButton;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private SwitchableElement _attentionPanel;
    [SerializeField] private SwitchableElement _menuPanel;
    [SerializeField] private Button _hotsit;

    [Header("Localization")]
    private readonly string _russian = "Russian";
    private readonly string _english = "English";
    private readonly string _turkish = "Turkish";
    [SerializeField] private Button _toEnglish;
    [SerializeField] private Button _toRussian;
    [SerializeField] private Button _toTurkish;

    private void Start()
    {
        _soundInitializer.Init();

        if (MusicSingleton.Instance.IsAdded == false)
            _soundInitializer.AddMusicSource(MusicSingleton.Instance.Music);
        else
            _soundInitializer.AddMusicSourceWithoutVolumeChanging(MusicSingleton.Instance.Music);

        SceneChangerSingleton.Instance.FadeOut();

        SaveSystem saveSystem = new SaveSystem();

        if (saveSystem.CanLoad)
        {
            _continueButton.interactable = true;
            _newGameButton.onClick.AddListener(OpenAttentionWindow);
        }
        else
        {
            _newGameButton.onClick.AddListener(CreateNewGame);
        }

        _confirmNewGameButton.onClick.AddListener(CreateNewGame);
        _hotsit.onClick.AddListener(OnHotsitButtonClick);

        LoadGameSingleton.Instance.Init();
        _continueButton.onClick.AddListener(OnContinueButtonClick);

        _toEnglish.onClick.AddListener(ChangeLanguageToEnglish);
        _toRussian.onClick.AddListener(ChangeLanguageToRussian);
        _toTurkish.onClick.AddListener(ChangeLanguageToTurkish);
    }

    private void OnDestroy()
    {
        _hotsit.onClick.RemoveListener(OnHotsitButtonClick);
        _newGameButton.onClick.RemoveAllListeners();
        _confirmNewGameButton.onClick.RemoveListener(CreateNewGame);
        _continueButton.onClick.RemoveListener(OnContinueButtonClick);
        _toEnglish.onClick.RemoveListener(ChangeLanguageToEnglish);
        _toRussian.onClick.RemoveListener(ChangeLanguageToRussian);
        _toTurkish.onClick.RemoveListener(ChangeLanguageToTurkish);
    }

    private void OnHotsitButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(Scenes.HotSitScene.ToString());
    }

    private void OpenAttentionWindow()
    {
        _attentionPanel.Enable();
        _menuPanel.Disable();
    }

    private void CreateNewGame()
    {
        SceneChangerSingleton.Instance.LoadScene(Scenes.GameScene.ToString());
    }

    private void ChangeLanguageToRussian()
    {
        SetLanguage(_russian);
    }

    private void ChangeLanguageToTurkish()
    {
        SetLanguage(_turkish);
    }

    private void ChangeLanguageToEnglish()
    {
        SetLanguage(_english);
    }

    private void SetLanguage(string language)
    {
        LeanLocalization.SetCurrentLanguageAll(language);
        LeanLocalization.UpdateTranslations();
    }

    private void OnContinueButtonClick()
    {
        LoadGameSingleton.Instance.ContinueGame();
        SceneChangerSingleton.Instance.LoadScene(Scenes.GameScene.ToString());
    }
}
