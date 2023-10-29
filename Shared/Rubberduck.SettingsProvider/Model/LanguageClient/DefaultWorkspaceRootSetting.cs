using System;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class DefaultWorkspaceRootSetting : TypedRubberduckSetting<Uri>
    {
        public static Uri DefaultSettingValue { get; } = new(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rubberduck", "Workspaces"));

        // TODO localize
        private static readonly string _description = "The default location for new projects hosted in a document that isn't saved yet.";

        public DefaultWorkspaceRootSetting()
            : base(nameof(DefaultWorkspaceRootSetting), DefaultSettingValue, SettingDataType.UriSetting, DefaultSettingValue) { }

        public DefaultWorkspaceRootSetting(Uri value)
            : base(nameof(DefaultWorkspaceRootSetting), value, SettingDataType.UriSetting, DefaultSettingValue) { }
    }
}
