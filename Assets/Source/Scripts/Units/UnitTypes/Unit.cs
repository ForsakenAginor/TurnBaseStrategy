using System;

public class Unit
{
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
    public event Action<Unit> Died;

    public int Health => _health.Amount;

    public int HealthMaximum => _health.Maximum;

    public Side Side => _side;

    public int CounterAttackPower => _counterAttackPower;

    public void TakeDamage(int amount) => _health.Spent(amount);

    private void OnResourceOver() => Died?.Invoke(this);

    private void OnHealthChanged() => HealthChanged?.Invoke();
}
