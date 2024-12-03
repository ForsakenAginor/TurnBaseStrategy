using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    private const string IsAttacking = nameof(IsAttacking);
    private const string IsDying = nameof(IsDying);
    private const string IsWalking = nameof(IsWalking);

    [SerializeField] private Animator _animator;

    public void Walk() => _animator.SetBool(IsWalking, true);

    public void Stop() => _animator.SetBool(IsWalking, false);

    public void Attack() => _animator.SetTrigger(IsAttacking);

    public void Die() => _animator.SetBool(IsDying, true);
}