using System;

public class Resource : IResource
{
    private int _amount;
    private int _maximum;

    public Resource(int amount)
    {
        _amount = amount >= 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount));
        _maximum = amount;
    }

    public event Action ResourcesAmountChanged;
    public event Action ResourceOver;

    public int Amount => _amount;

    public int Maximum => _maximum;

    public void Add(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        int temp = _amount + amount;
        _amount = Math.Min(_maximum, temp);
        ResourcesAmountChanged?.Invoke();
    }

    public bool TrySpent(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (_amount < amount)
            return false;

        _amount -= amount;
        ResourcesAmountChanged?.Invoke();
        return true;
    }

    public void Spent(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        _amount -= amount;
        ResourcesAmountChanged?.Invoke();

        if (_amount <= 0)
            ResourceOver?.Invoke();
    }
}