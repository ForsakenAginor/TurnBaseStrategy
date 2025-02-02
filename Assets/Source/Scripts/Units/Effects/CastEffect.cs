using DG.Tweening;
using UnityEngine;

public class CastEffect : UnitEffect
{
    [SerializeField] private Transform _aura;
    [SerializeField] private ParticleSystem _lightning;
    [SerializeField] private float _duration = 1f;

    private Vector3 _startScale = new Vector3(1f, 0.1f, 1f);

    public override void PlayEffect(Vector3 start, Vector3 end)
    {
        _aura.gameObject.SetActive(true);
        _lightning.transform.position = end;
        _aura.transform.localScale = _startScale;
        _aura.transform.DOScale(Vector3.one, _duration).OnComplete(SecondPart);
    }

    private void SecondPart()
    {
        _lightning.Play();
        _aura.gameObject.SetActive(false);
    }
}
