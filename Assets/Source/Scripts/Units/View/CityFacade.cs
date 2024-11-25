public class CityFacade : UnitFacade, ICityFacade
{
    private CityMenu _cityMenu;

    public CityMenu Menu => _cityMenu;

    protected override void InitializeFieldsInAwake()
    {
        _cityMenu = GetComponent<CityMenu>();
    }
}
