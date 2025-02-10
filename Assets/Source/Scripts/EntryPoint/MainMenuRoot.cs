using Assets.Scripts.General;
using Assets.Scripts.Sound.AudioMixer;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuRoot : MonoBehaviour
{
    [SerializeField] private SoundInitializer _soundInitializer;
    [SerializeField] private Button _continueButton;

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

        if(saveSystem.CanLoad)
            _continueButton.interactable = true;

        LoadGameSingleton.Instance.Init();
        _continueButton.onClick.AddListener(OnContinueButtonClick);

        _toEnglish.onClick.AddListener(ChangeLanguageToEnglish);
        _toRussian.onClick.AddListener(ChangeLanguageToRussian);
        _toTurkish.onClick.AddListener(ChangeLanguageToTurkish);
    }

    private void OnDestroy()
    {
        _continueButton.onClick.RemoveListener(OnContinueButtonClick);
        _toEnglish.onClick.RemoveListener(ChangeLanguageToEnglish);
        _toRussian.onClick.RemoveListener(ChangeLanguageToRussian);
        _toTurkish.onClick.RemoveListener(ChangeLanguageToTurkish);
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
