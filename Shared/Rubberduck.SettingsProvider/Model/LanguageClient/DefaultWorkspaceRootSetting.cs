using System;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class DefaultWorkspaceRootSetting : RubberduckSetting<Uri>
    {
        public static Uri DefaultSettingValue { get; } = new(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rubberduck", "Workspaces"));

        // TODO localize
        private static readonly string _description = "The default location for new projects hosted in a document that isn't saved yet.";

        public DefaultWorkspaceRootSetting()
            : base(SettingDataType.UriSetting, nameof(DefaultWorkspaceRootSetting), _description, DefaultSettingValue)
        {
        }

        public DefaultWorkspaceRootSetting(Uri value)
            : base(SettingDataType.UriSetting, nameof(DefaultWorkspaceRootSetting), _description, DefaultSettingValue, value)
        {
        }
    }
}
