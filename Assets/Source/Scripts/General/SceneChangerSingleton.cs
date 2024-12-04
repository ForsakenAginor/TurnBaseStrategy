using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.General
{
    public class SceneChangerSingleton : MonoBehaviour
    {
        private static SceneChangerSingleton _instance;

        [SerializeField] private Image _blackScreenImage;
        [SerializeField] private float _animationDuration;

        public static SceneChangerSingleton Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _blackScreenImage.gameObject.SetActive(true);
            _blackScreenImage.color = Color.black;
        }

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                throw new ArgumentNullException(nameof(sceneName));

            _blackScreenImage.DOFade(1f, _animationDuration).OnComplete(() => StartCoroutine(LoadAsyncScene(sceneName)));
        }

        public void FadeOut()
        {
            _blackScreenImage.DOFade(0f, _animationDuration);
        }

        private IEnumerator LoadAsyncScene(string scene)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

            while (asyncLoad.isDone == false)
                yield return null;
        }
    }
}