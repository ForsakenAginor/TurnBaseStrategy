using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Sound
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSoundHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource _audio;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _audio.Play();
        }
    }
}