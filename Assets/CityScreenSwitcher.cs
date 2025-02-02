using UnityEngine;

public class CityScreenSwitcher : MonoBehaviour
{
    [SerializeField] private UIElement _buttons;

    private void OnEnable()
    {
        _buttons.Disable();
    }

    private void OnDisable()
    {
        _buttons.Enable();
    }
}
