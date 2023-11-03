using System;

namespace Rubberduck.SettingsProvider.Model
{
    public record class UriRubberduckSetting : TypedRubberduckSetting<Uri>
    {
        public UriRubberduckSetting()
        {
            SettingDataType = SettingDataType.UriSetting;
        }
    }
}
