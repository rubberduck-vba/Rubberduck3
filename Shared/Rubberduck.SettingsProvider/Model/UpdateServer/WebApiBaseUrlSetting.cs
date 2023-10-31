using System;

namespace Rubberduck.SettingsProvider.Model.UpdateServer
{
    /// <summary>
    /// Determines the base address of the web API that responds to Rubberduck version checks.
    /// </summary>
    public class WebApiBaseUrlSetting : UriRubberduckSetting
    {
        public static Uri DefaultSettingValue { get; } = new("https://api.rubberduckvba.com/api/v1");

        public WebApiBaseUrlSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
