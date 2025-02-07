using UnityEngine;

public class SwitchableElement : MonoBehaviour, ISwitchableElement
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