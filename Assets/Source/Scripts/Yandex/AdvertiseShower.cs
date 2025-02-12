using Agava.YandexGames;
using System;
using UnityEngine;

public class AdvertiseShower
{
    private readonly Silencer _silencer;
    private Action _callback;

    public AdvertiseShower(Silencer silencer)
    {
        _silencer = silencer != null ? silencer : throw new ArgumentNullException(nameof(silencer));
    }

    public void ShowAdvertise(Action callback)
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        AudioListener.volume = 0f;
        _callback = callback;
        InterstitialAd.Show(null, OnCloseAdvertise);
    }

    private void OnCloseAdvertise(bool _)
    {
        AudioListener.pause = false;
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
        _silencer.SetGameState(Time.timeScale, AudioListener.volume, AudioListener.pause);
        _callback?.Invoke();
    }
}
