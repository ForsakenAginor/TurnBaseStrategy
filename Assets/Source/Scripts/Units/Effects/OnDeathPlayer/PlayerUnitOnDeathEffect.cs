using DG.Tweening;
using UnityEngine;

public class PlayerUnitOnDeathEffect : MonoBehaviour, IUIElement
{
    private const string MaterialAlpha = "_AlphaCliping";

    [SerializeField] private UIElement _fire;
    [SerializeField] private float _duration;
    [SerializeField] private float _durationDissolve;
    [SerializeField] private SkinnedMeshRenderer _renderer;

    private Material _material;

    private void Awake()
    {
        _material = new Material(_renderer.material);
        _renderer.material = _material;
        _material.SetFloat(MaterialAlpha, 1f);
    }

    private void Start()
    {
        _material.DOFloat(0f, MaterialAlpha, 1).SetEase(Ease.Linear);        
    }

    public void Disable()
    {
        _fire.Disable();
    }

    public void Enable()
    {
        _fire.Enable();
        _fire.transform.DOScale(Vector3.one, _duration).SetLoops(2, LoopType.Yoyo);
        _material.DOFloat(1f, MaterialAlpha, _durationDissolve).SetEase(Ease.Linear);
    }
}
