using System;

namespace Rubberduck.InternalApi.Settings.Model;

public record class UriRubberduckSetting : TypedRubberduckSetting<Uri>
{
    public UriRubberduckSetting()
    {
        SettingDataType = SettingDataType.UriSetting;
    }
}
