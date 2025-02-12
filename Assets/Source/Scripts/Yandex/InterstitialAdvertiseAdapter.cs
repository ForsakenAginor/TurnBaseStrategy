using System;

public class InterstitialAdvertiseAdapter
{
    private const int AdvertiseFrequency = 10;

    private readonly AdvertiseShower _advertise;
    private readonly ISavedDaySystem _daySystem;

    public InterstitialAdvertiseAdapter(Silencer silencer, ISavedDaySystem daySystem)
    {
        if ( silencer == null )
            throw new ArgumentNullException(nameof(silencer));

        _advertise = new AdvertiseShower(silencer);
        _daySystem = daySystem != null ? daySystem : throw new ArgumentNullException(nameof(daySystem));

        _daySystem.DayChanged += OnDayChanged;
    }

    ~InterstitialAdvertiseAdapter()
    {
        _daySystem.DayChanged -= OnDayChanged;
    }

    private void OnDayChanged(int day)
    {
        if(day % AdvertiseFrequency == 0)
            _advertise.ShowAdvertise(null);
    }
}