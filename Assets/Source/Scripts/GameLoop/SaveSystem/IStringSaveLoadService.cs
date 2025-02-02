public interface IStringSaveLoadService
{
    public bool CanLoad { get; }

    public string GetSavedInfo();

    public void SaveInfo(string value);
}
