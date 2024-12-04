using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class UIWindowSwitcher : MonoBehaviour
    {
        [SerializeField] private UIElement _targetWindow;
        [SerializeField] private UIElement _currentWindow;

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
            _targetWindow.Enable();
            _currentWindow.Disable();
        }
    }
}