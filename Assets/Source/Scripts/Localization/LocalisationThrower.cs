using Sirenix.OdinInspector;
using UnityEngine;

namespace Localization
{
    public class LocalisationThrower : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

#if UNITY_EDITOR
        [Button]
        private void ToEnglish()
        {
            string language = "en";
            LocalizationInitializer localizationInitializer = new();
            localizationInitializer.ApplyLocalization(language);
        }

        [Button]
        private void ToRussian()
        {
            string language = "ru";
            LocalizationInitializer localizationInitializer = new();
            localizationInitializer.ApplyLocalization(language);
        }

        [Button]
        private void ToTurkish()
        {
            string language = "tr";
            LocalizationInitializer localizationInitializer = new();
            localizationInitializer.ApplyLocalization(language);
        }
#endif
    }
}