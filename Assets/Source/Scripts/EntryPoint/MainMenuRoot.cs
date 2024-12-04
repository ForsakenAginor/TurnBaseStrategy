using Assets.Scripts.General;
using Assets.Scripts.Sound.AudioMixer;
using UnityEngine;

public class MainMenuRoot : MonoBehaviour
{
    [SerializeField] private SoundInitializer _soundInitializer;

    private void Start()
    {
        _soundInitializer.Init();

        if (MusicSingleton.Instance.IsAdded == false)
            _soundInitializer.AddMusicSource(MusicSingleton.Instance.Music);
        else
            _soundInitializer.AddMusicSourceWithoutVolumeChanging(MusicSingleton.Instance.Music);

        SceneChangerSingleton.Instance.FadeOut();
    }
}
