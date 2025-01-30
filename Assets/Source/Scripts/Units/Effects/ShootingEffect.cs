using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShootingEffect : UnitEffect
{
    [SerializeField] private float _height;
    [SerializeField] private float _halfDuration;
    [SerializeField] private ParticleSystem _fire;
    [SerializeField] private ParticleSystem _smoke;

    private Vector3 _position;

    private void OnEnable()
    {
        _fire.Play();
        _smoke.Play();
    }

    private void OnDisable()
    {
        _fire.Stop();
        _smoke.Stop();
    }

    private void FixedUpdate()
    {

        if (gameObject.activeSelf == false)
            return;

        var direction = (transform.position - _position) + transform.position;
        transform.LookAt(direction);
        _position = transform.position;
    }

    [Button]
    public override void PlayEffect(Vector3 start, Vector3 end)
    {
        gameObject.SetActive(true);
        transform.position = start;
        transform.DOMove(end, _halfDuration * 2f).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
        transform.DOMoveY(end.y + _height, _halfDuration).SetLoops(2, LoopType.Yoyo);
    }
}

public abstract class UnitEffect : MonoBehaviour
{
    public abstract void PlayEffect(Vector3 start, Vector3 end);
}
