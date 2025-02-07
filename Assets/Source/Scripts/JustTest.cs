using Sirenix.OdinInspector;
using UnityEngine;

public class JustTest : MonoBehaviour
{
    [SerializeField] private Transform _prefab;

    [Button]
    private void JustDoIt()
    {
        var parent = GetComponentInParent<HexOnScene>();
        var good = Instantiate(_prefab, parent.transform);
        gameObject.transform.SetParent(good);
    }

    [Button]
    private void JustDoItAnotherTime()
    {
        var parent = GetComponentInParent<HexOnScene>();
        var evil = parent.GetComponentInChildren<EvilHex>();
        var content = Instantiate(gameObject, evil.transform);
        content.name = "EvilContent";
    }
}
