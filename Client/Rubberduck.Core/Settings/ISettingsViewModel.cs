namespace Rubberduck.Core.Settings
{
    public interface ISettingsViewModel<TSettings>
    {
        TSettings ToSettings();
    }
}
