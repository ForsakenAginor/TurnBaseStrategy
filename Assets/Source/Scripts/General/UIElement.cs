using UnityEngine;

public class UIElement : MonoBehaviour, IUIElement
{
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
            gameObject.SetActive(false);
    }
}