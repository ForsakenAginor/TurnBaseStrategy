using Lean.Localization;
using TMPro;
using UnityEngine;

public class CityName : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameField;

    private string _name;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.U))
            SetName();
    }

    public void Init(string name)
    {
        _name = name;
        SetName();
    }

    private void SetName()
    {
        string cityName = LeanLocalization.GetTranslationText(_name);
        _nameField.text = cityName;
    }
}
