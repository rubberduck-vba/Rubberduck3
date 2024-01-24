using Rubberduck.InternalApi.Settings.Model;
using System;

namespace Rubberduck.InternalApi.Settings.Model.UpdateServer
{
    /// <summary>
    /// Determines the base address of the web API that responds to Rubberduck version checks.
    /// </summary>
    public record class WebApiBaseUrlSetting : UriRubberduckSetting
    {
        public static Uri DefaultSettingValue { get; } = new("https://api.rubberduckvba.com/api/v1");

        public WebApiBaseUrlSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
