namespace Rubberduck.SettingsProvider.Model
{
    public interface IDefaultSettingsProvider<TSettings>
    {
        TSettings Default { get; }
    }

    public class DefaultLanguageServerSettingsProvider : IDefaultSettingsProvider<LanguageServerSettings>
    {
        public LanguageServerSettings Default { get; } = LanguageServerSettings.Default;
    }

    public class DefaultUpdateServerSettingsProvider : IDefaultSettingsProvider<UpdateServerSettings>
    {
        public UpdateServerSettings Default { get; } = UpdateServerSettings.Default;
    }
}
