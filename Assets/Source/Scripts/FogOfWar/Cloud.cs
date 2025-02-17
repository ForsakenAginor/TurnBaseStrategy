using DG.Tweening;
using UnityEngine;

public class Cloud : MonoBehaviour, ICloud
{
    [SerializeField] private float _duration = 0.5f;
    private bool _isDissappeared;
    private SwitchableElement _holder;

    public SwitchableElement Holder => _holder;

    public bool IsDissappeared => _isDissappeared;

    public void Disappear()
    {
        transform.DOScale(Vector3.zero, _duration).OnComplete(Remove);
        _isDissappeared = true;
    }

    public void Init()
    {
        _holder = transform.parent.GetComponent<SwitchableElement>();
    }

    private void Remove()
    {
        Destroy(gameObject, _duration);
    }
}

public interface ICloud
{
    public SwitchableElement Holder { get; }

    public bool IsDissappeared { get; }

    public void Disappear();
}
