using Lean.Localization;
using TMPro;
using UnityEngine;

public class CityName : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameField;

    public void Init(string name)
    {
        string cityName = LeanLocalization.GetTranslationText(name);
        _nameField.text = cityName;
    }
}
