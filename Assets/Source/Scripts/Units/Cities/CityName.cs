using TMPro;
using UnityEngine;

public class CityName : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameField;

    public void Init(string name)
    {
        _nameField.text = name;
    }
}
