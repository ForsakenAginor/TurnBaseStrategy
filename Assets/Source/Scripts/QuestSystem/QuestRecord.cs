using Lean.Localization;
using TMPro;
using UnityEngine;

public class QuestRecord : MonoBehaviour
{
    private const string Capture = "CAPTURE";

    [SerializeField] private SwitchableElement _completeImage;
    [SerializeField] private SwitchableElement _nonCompleteImage;
    [SerializeField] private TMP_Text _text;

    public void Init(string name, bool isCompleted)
    {
        if (isCompleted)
            _completeImage.Enable();
        else
            _nonCompleteImage.Enable();


        string capture = LeanLocalization.GetTranslationText(Capture);
        string cityName = LeanLocalization.GetTranslationText(name);
        _text.text = $"{capture} {cityName}";
    }
}
