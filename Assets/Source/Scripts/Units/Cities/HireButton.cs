using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HireButton : MonoBehaviour
{
    [SerializeField] private Image _coinImage;
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Button _button;

    private Color _hideColor = new(0.8f, 0.8f, 0.8f, 0.6f);
    private Color _color = Color.white;

    public Button HireUnitButton => _button;

    [Button]
    public void DeActivate()
    {
        _button.interactable = false;
        _coinImage.gameObject.SetActive(false);
        _unitIcon.color = _hideColor;
    }

    [Button]
    public void Activate()
    {
        _button.interactable = true;
        _coinImage.gameObject.SetActive(true);
        _unitIcon.color = _color;
    }
}
