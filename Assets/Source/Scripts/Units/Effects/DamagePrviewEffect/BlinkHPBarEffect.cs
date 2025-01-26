using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlinkHPBarEffect : MonoBehaviour
{
    [SerializeField] private Slider _bottomSlider;
    [SerializeField] private Slider _topSlider;
    [SerializeField] private Image _image;
    [SerializeField] private Color _first;
    [SerializeField] private Color _next;
    [SerializeField] private float _frequence;

    private Tween _tween;

    public void PlayEffect(float max, float current, float next)
    {
        _tween?.Kill();
        _image.color = _first;
        _bottomSlider.value = current / max;
        _topSlider.value = next / max;
        _tween = _image.DOColor(_next, _frequence).SetLoops(-1, LoopType.Yoyo);
    }
}
