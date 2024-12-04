using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.General
{
    [RequireComponent(typeof(Button))]
    public class SceneChangeButton : MonoBehaviour
    {
        [SerializeField] private Scenes _scene;
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
            SceneChangerSingleton.Instance.LoadScene(_scene.ToString());
        }
    }
}