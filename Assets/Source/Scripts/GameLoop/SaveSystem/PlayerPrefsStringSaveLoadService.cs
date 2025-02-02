using System;
using UnityEngine;

public class PlayerPrefsStringSaveLoadService : IStringSaveLoadService
{
    private readonly string _playerPrefsKey = nameof(_playerPrefsKey);

    public PlayerPrefsStringSaveLoadService(string key)
    {
        if(string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        _playerPrefsKey = key;
    }

    public bool CanLoad => PlayerPrefs.HasKey(_playerPrefsKey);

    public string GetSavedInfo()
    {
        if (PlayerPrefs.HasKey(_playerPrefsKey) == false)
            throw new Exception($"Can't find {_playerPrefsKey} data in playerPrefs");

        return PlayerPrefs.GetString(_playerPrefsKey);
    }

    public void SaveInfo(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        PlayerPrefs.SetString(_playerPrefsKey, value);
    }
}
