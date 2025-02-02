using Assets.Scripts.General;
using Assets.Scripts.Sound.AudioMixer;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuRoot : MonoBehaviour
{
    [SerializeField] private SoundInitializer _soundInitializer;
    [SerializeField] private Button _continueButton;

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

        _continueButton.onClick.AddListener(OnContinueButtonClick);
    }

    private void OnDestroy()
    {
        _continueButton.onClick.RemoveListener(OnContinueButtonClick);
    }

    private void OnContinueButtonClick()
    {
        LoadGameSingleton.Instance.ContinueGame();
        SceneChangerSingleton.Instance.LoadScene(Scenes.GameScene.ToString());
    }
}
