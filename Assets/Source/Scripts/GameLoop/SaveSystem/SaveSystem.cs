using System;

public class SaveSystem
{
    private const string LevelData = nameof(LevelData);
    private const int SaveFrequency = 10;

    private readonly DataStorage<SavedData> _storage;

    private ISavedFogOfWar _fogOfWar;
    private ISavedUnits _units;
    private ISavedCities _cities;
    private ISavedWallet _wallet;
    private ISavedDaySystem _daySystem;
    private ISavedScaner _scaner;
    private GameLevel _level;
    private bool _isInited;

    public SaveSystem()
    {
        _storage = new(LevelData);
    }

    ~SaveSystem()
    {
        if (_isInited)
            _daySystem.DayChanged -= OnDayChanged;
    }

    public bool CanLoad => _storage.CanLoad;

    public void Init(ISavedFogOfWar fogOfWar, ISavedUnits units, ISavedCities cities,
        ISavedWallet wallet, ISavedDaySystem day, GameLevel level, ISavedScaner scaner)
    {
        if (_isInited)
            throw new InvalidOperationException("SaveSystem already inited");

        _fogOfWar = fogOfWar != null ? fogOfWar : throw new ArgumentNullException(nameof(fogOfWar));
        _units = units != null ? units : throw new ArgumentNullException(nameof(units));
        _cities = cities != null ? cities : throw new ArgumentNullException(nameof(cities));
        _wallet = wallet != null ? wallet : throw new ArgumentNullException(nameof(wallet));
        _daySystem = day != null ? day : throw new ArgumentNullException(nameof(day));
        _scaner = scaner != null ? scaner : throw new ArgumentNullException(nameof(scaner));
        _level = level;

        _daySystem.DayChanged += OnDayChanged;

        _isInited = true;
    }


    public void Save()
    {
        if (_isInited == false)
            throw new Exception("SaveSystem not inited yet");

        SavedData data = new SavedData(_fogOfWar.DiscoveredCells, _wallet.Amount, _daySystem.CurrentDay,
            _units.GetInfo(), _cities.GetInfo(), _level, _scaner.Cities);
        _storage.SaveData(data);
    }

    public SavedData Load()
    {
        return _storage.LoadData();
    }

    private void OnDayChanged(int currentDay)
    {
        if (currentDay % SaveFrequency == 0)
            Save();
    }
}
