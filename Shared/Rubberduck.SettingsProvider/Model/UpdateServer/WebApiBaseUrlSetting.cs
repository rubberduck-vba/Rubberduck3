﻿using System;

namespace Rubberduck.SettingsProvider.Model
{
    public record class WebApiBaseUrlSetting : TypedRubberduckSetting<Uri>
    {
        // TODO localize
        private static readonly string _description = "Determines the base address of the web API that responds to Rubberduck version checks.";

        public WebApiBaseUrlSetting(Uri defaultValue) : this(defaultValue, defaultValue) { }
        public WebApiBaseUrlSetting(Uri defaultValue, Uri value)
            : base(nameof(WebApiBaseUrlSetting), value, SettingDataType.UriSetting, defaultValue, readOnlyRecommended: true) { }
    }
}
