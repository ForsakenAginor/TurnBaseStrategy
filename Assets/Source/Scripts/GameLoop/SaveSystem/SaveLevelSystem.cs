using UnityEngine;

public class SaveLevelSystem
{
    private const string Level = nameof(Level);

    public GameLevel LoadLevel()
    {
        if (PlayerPrefs.HasKey(Level) == false)
            return GameLevel.First;

        return (GameLevel)PlayerPrefs.GetInt(Level);
    }

    public void SaveLevel(GameLevel value)
    {
        PlayerPrefs.SetInt(Level, (int)value);
    }
}
