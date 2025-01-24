using DG.Tweening;
using UnityEngine;

public class Cloud : MonoBehaviour, ICloud
{
    [SerializeField] private float _duration = 0.5f;
    private bool _isDissappeared;

    public bool IsDissappeared => _isDissappeared;

    public void Disappear()
    {
        transform.DOScale(Vector3.zero, _duration).OnComplete(Remove);
        _isDissappeared = true;
    }

    private void Remove()
    {
        Destroy(gameObject, _duration);
    }
}

public interface ICloud
{
    public bool IsDissappeared { get; }

    public void Disappear();
}
