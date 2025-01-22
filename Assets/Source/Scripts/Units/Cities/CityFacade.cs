using UnityEngine;

[RequireComponent(typeof(CityName))]
[RequireComponent(typeof(CityMenu))]
public class CityFacade : UnitFacade, ICityFacade
{
    private CityMenu _cityMenu;
    private CityName _cityName;

    public CityMenu Menu => _cityMenu;

    public CityName CityName => _cityName;

    protected override void InitializeFieldsInAwake()
    {
        _cityMenu = GetComponent<CityMenu>();
        _cityName = GetComponent<CityName>();
    }
}