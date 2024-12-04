using Assets.Scripts.General;
using UnityEngine;
using UnityEngine.UI;

public class EndGameButtonInitializer : MonoBehaviour
{
    [SerializeField] private Button _loseButton;
    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _winButton;

    private void Awake()
    {
        _loseButton.onClick.AddListener(OnLoseButtonClick);
        _finishButton.onClick.AddListener(OnFinishButtonClick);
        _winButton.onClick.AddListener(OnWinButtonClick);
    }

    private void OnDestroy()
    {
        _loseButton.onClick.RemoveListener(OnLoseButtonClick);
        _finishButton.onClick.RemoveListener(OnFinishButtonClick);
        _winButton.onClick.RemoveListener(OnWinButtonClick);
    }

    private void OnWinButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(Scenes.GameScene.ToString());
    }

    private void OnFinishButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(Scenes.MenuScene.ToString());
    }

    private void OnLoseButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(Scenes.GameScene.ToString());
    }
}