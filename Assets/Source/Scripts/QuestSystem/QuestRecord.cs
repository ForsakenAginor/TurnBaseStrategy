using TMPro;
using UnityEngine;

public class QuestRecord : MonoBehaviour
{
    private const string Content = "Capture ";

    [SerializeField] private UIElement _completeImage;
    [SerializeField] private UIElement _nonCompleteImage;
    [SerializeField] private TMP_Text _text;

    public void Init(string name, bool isCompleted)
    {
        if (isCompleted)
            _completeImage.Enable();
        else
            _nonCompleteImage.Enable();

        _text.text = $"{Content}{name}";
    }
}
