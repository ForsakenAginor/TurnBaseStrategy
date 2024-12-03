using UnityEngine;

namespace Assets.Scripts.Sound.AudioMixer
{
    public class MusicSingleton : MonoBehaviour
    {
        private static MusicSingleton _instance;
        [SerializeField] private AudioSource _music;
        private bool _isAdded;

        public static MusicSingleton Instance => _instance;

        public AudioSource Music
        {
            get
            {
                _isAdded = true;
                return _music;
            }
        }

        public bool IsAdded => _isAdded;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}