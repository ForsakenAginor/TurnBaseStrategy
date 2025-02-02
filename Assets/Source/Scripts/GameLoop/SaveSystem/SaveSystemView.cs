using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystemView : MonoBehaviour, IControllable
{
    [SerializeField] private Button _saveButton;

    private SaveSystem _saveSystem;

    private void OnDestroy()
    {
        _saveButton.onClick.RemoveListener(SaveGameData);
    }

    public void DisableControl()
    {
        _saveButton.interactable = false;
    }

    public void EnableControl()
    {
        _saveButton.interactable = true;
    }

    public void Init(SaveSystem saveSystem)
    {
        _saveSystem = saveSystem != null ? saveSystem : throw new ArgumentNullException(nameof(saveSystem));
        _saveButton.onClick.AddListener(SaveGameData);
    }

    private void SaveGameData()
    {
        _saveButton.interactable = false;
        _saveSystem.Save();
    }
}
