using UnityEngine;

public class LoadGameSingleton : MonoBehaviour
{
    private bool _isContinueGame;
    private static LoadGameSingleton _instance;

    public static LoadGameSingleton Instance => _instance;

    public bool IsContinueGame => _isContinueGame;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ContinueGame() => _isContinueGame = true;

    public void Init() => _isContinueGame = false;
}