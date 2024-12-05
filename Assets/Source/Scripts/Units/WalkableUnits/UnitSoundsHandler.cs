using System;
using UnityEngine;

public class UnitSoundsHandler : MonoBehaviour
{
    [SerializeField] private AudioClip _walking;
    [SerializeField] private AudioClip _attacking;
    [SerializeField] private AudioClip _diying;
    [SerializeField] private AudioClip _hiring;
    [SerializeField] private AudioSource _audioSource;

    public void Init(Action<AudioSource> callback)
    {
        if(callback == null)
            throw new ArgumentNullException(nameof(callback));

        callback.Invoke(_audioSource);
    }

    public void Walk() => PlayClip(_walking);

    public void Dying() => PlayClip(_diying);

    public void Attack() => PlayClip(_attacking);

    public void Hire() => PlayClip(_hiring);

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
