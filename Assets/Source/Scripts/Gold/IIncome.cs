using System;
using System.Collections.Generic;

public interface IIncome
{
    public event Action<int> IncomeChanged;

    public event Action<List<KeyValuePair<string, int>>> IncomeCompositionChanged;
}
