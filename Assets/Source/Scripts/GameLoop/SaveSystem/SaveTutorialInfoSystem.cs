using UnityEngine;

public class SaveTutorialInfoSystem
{
    private const string Tutorial = nameof(Tutorial);

    public bool IsTutorialComplete()
    {
        return PlayerPrefs.HasKey(Tutorial) && PlayerPrefs.GetInt(Tutorial) == 1;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(Tutorial, 1);
    }
}
