using Rubberduck.InternalApi.Settings.Model;
using System;
using System.IO;

namespace Rubberduck.InternalApi.Settings.Model.LanguageClient;

/// <summary>
/// The default location for new projects hosted in a document that isn't saved yet.
/// </summary>
public record class DefaultWorkspaceRootSetting : UriRubberduckSetting
{
    private static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public static Uri DefaultSettingValue { get; } = new(Path.Combine(LocalAppData, "Rubberduck", "Workspaces"));

    public DefaultWorkspaceRootSetting()
    {
        DefaultValue = DefaultSettingValue;
        Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended;
    }
}
