namespace Rubberduck.InternalApi.Settings
{
    /// <summary>
    /// Abstracts file I/O operations for a provided <c>TSettings</c> type.
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public interface ISettingsService<TSettings> : ISettingsProvider<TSettings>, ISettingsChangedHandler<TSettings>
    {
        /// <summary>
        /// Reads and deserializes settings from disk into a <c>TSettings</c> value.
        /// </summary>
        /// <returns></returns>
        TSettings Read();

        /// <summary>
        /// Serializes the provided <c>TSettings</c> value to disk.
        /// </summary>
        void Write(TSettings settings);

        //void Write<TSettingGroup>(TSettingGroup settings) where TSettingGroup : TypedSettingGroup;
    }
}