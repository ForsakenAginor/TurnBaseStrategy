using DG.Tweening;
using UnityEngine;

public class PlayerUnitOnDeathEffect : MonoBehaviour, ISwitchableElement
{
    private const string MaterialAlpha = "_AlphaCliping";

    [SerializeField] private SwitchableElement _fire;
    [SerializeField] private float _duration;
    [SerializeField] private float _durationDissolve;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Material _standardMaterial;

    private Material _material;
    private bool _isMobile;

    private void Awake()
    {
        _isMobile = Application.isMobilePlatform &&
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android);

        _material = _isMobile == false ? new Material(_renderer.material) : new Material(_standardMaterial);
        _renderer.material = _material;

        if (_isMobile == false)
            _material.SetFloat(MaterialAlpha, 1f);
    }

    private void Start()
    {
        if (_isMobile == false)
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

        if (_isMobile == false)
            _material.DOFloat(1f, MaterialAlpha, _durationDissolve).SetEase(Ease.Linear);
    }
}
