using UnityEngine;

public class HexOnScene : MonoBehaviour, IBlockedCell
{
    [SerializeField] private bool _isBlocked;

    public bool IsBlocked => _isBlocked;
}

public interface IBlockedCell
{
    public bool IsBlocked { get;}
}
