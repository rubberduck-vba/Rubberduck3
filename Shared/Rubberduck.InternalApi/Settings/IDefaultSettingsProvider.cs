namespace Rubberduck.InternalApi.Settings;

public interface IDefaultSettingsProvider<TSettings>
{
    TSettings Default { get; }
}
