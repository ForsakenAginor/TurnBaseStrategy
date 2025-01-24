using DG.Tweening;
using UnityEngine;

public class PlayerUnitOnDeathEffect : MonoBehaviour, IUIElement
{
    [SerializeField] private UIElement _fire;
    [SerializeField] private float _duration;
    [SerializeField] private float _durationDissolve;
    [SerializeField] private SkinnedMeshRenderer _renderer;

    private Material _material;

    private void Awake()
    {
        _material = new Material(_renderer.material);
        _renderer.material = _material;
    }

    public void Disable()
    {
        _fire.Disable();
    }

    public void Enable()
    {
        _fire.Enable();
        _fire.transform.DOScale(Vector3.one, _duration).SetLoops(2, LoopType.Yoyo);
        _material.DOFloat(1f, "_AlphaCliping", _durationDissolve).SetEase(Ease.Linear);
    }
}
