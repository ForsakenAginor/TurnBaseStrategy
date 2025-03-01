using Assets.Scripts.General;
using System;
using UnityEngine;
using UnityEngine.UI;

public class EndGameButtonInitializer : MonoBehaviour
{
    [SerializeField] private Button _loseButton;
    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _winButton;
    [SerializeField] private Button _disconnectedButton;
    [SerializeField] private Scenes _winScene;
    [SerializeField] private Scenes _finishScene;
    [SerializeField] private Scenes _loseScene;
    [SerializeField] private Scenes _menuScene;

    private void Awake()
    {
        _loseButton.onClick.AddListener(OnLoseButtonClick);
        _finishButton.onClick.AddListener(OnFinishButtonClick);
        _winButton.onClick.AddListener(OnWinButtonClick);
        _disconnectedButton?.onClick.AddListener(OnDisconnectionButtonClick);
    }

    private void OnDestroy()
    {
        _loseButton.onClick.RemoveListener(OnLoseButtonClick);
        _finishButton.onClick.RemoveListener(OnFinishButtonClick);
        _winButton.onClick.RemoveListener(OnWinButtonClick);
        _disconnectedButton?.onClick.RemoveListener(OnDisconnectionButtonClick);
    }

    private void OnDisconnectionButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(_menuScene.ToString());
    }

    private void OnWinButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(_winScene.ToString());
    }

    private void OnFinishButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(_finishScene.ToString());
    }

    private void OnLoseButtonClick()
    {
        SceneChangerSingleton.Instance.LoadScene(_loseScene.ToString());
    }
}