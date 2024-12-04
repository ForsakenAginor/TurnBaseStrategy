using UnityEngine;

public class SaveSystem
{
    private const string Level = nameof(Level);

    public GameLevel Load()
    {
        if (PlayerPrefs.HasKey(Level) == false)
            return GameLevel.First;

        return (GameLevel)PlayerPrefs.GetInt(Level);
    }

    public void Save(GameLevel value)
    {
        PlayerPrefs.SetInt(Level, (int)value);
    }
}
