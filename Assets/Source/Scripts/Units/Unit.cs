using System;

public class Unit
{
    private const int LackOfPaymentDamage = 3;
    private const int HealingValue = 2;

    private readonly Side _side;
    private readonly Resource _health;
    private readonly int _counterAttackPower;

    public Unit(Side side, Resource health, int counterAttackPower)
    {
        _side = side;
        _health = health != null ? health : throw new ArgumentNullException(nameof(health));
        _counterAttackPower = counterAttackPower >= 0 ? counterAttackPower : throw new ArgumentOutOfRangeException(nameof(counterAttackPower));

        _health.ResourcesAmountChanged += OnHealthChanged;
        _health.ResourceOver += OnResourceOver;
    }

    ~Unit()
    {
        _health.ResourcesAmountChanged -= OnHealthChanged;
        _health.ResourceOver -= OnResourceOver;
    }

    public event Action HealthChanged;
    public event Action<Unit> Destroyed;
    public event Action<Unit> Captured;

    public int Health => _health.Amount;

    public bool IsAlive => _health.Amount > 0;

    public int HealthMaximum => _health.Maximum;

    public Side Side => _side;

    public int CounterAttackPower => _counterAttackPower;

    public void TakeDamage(int amount) => _health.Spent(amount);

    public void SufferPaymentDamage() => TakeDamage(LackOfPaymentDamage);

    public void HealingUnit() => _health.Add(HealingValue);

    protected void Kill() => Destroyed?.Invoke(this);

    private void OnResourceOver()
    {
        Captured?.Invoke(this);
        Destroyed?.Invoke(this);
    }

    private void OnHealthChanged() => HealthChanged?.Invoke();
}
