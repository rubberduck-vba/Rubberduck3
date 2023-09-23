namespace Rubberduck.SettingsProvider
{
    public interface ISettingsProvider<TSettings>
    {
        TSettings Value { get; }
    }
}
