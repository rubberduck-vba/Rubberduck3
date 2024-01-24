using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.ServerPlatform;
using System;
using System.Diagnostics;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Rubberduck.ServerPlatform
{
    public class WorkspaceRootResolver
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<RubberduckSettings> _settings;

        public WorkspaceRootResolver(ILogger logger, ISettingsProvider<RubberduckSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        protected TraceLevel TraceLevel => _settings.Settings.LoggerSettings.TraceLevel.ToTraceLevel();

        public Uri GetWorkspaceRootUri(ServerStartupOptions options)
        {
            var settings = _settings.Settings.LanguageClientSettings;
            var setting = settings.WorkspaceSettings.GetSetting<DefaultWorkspaceRootSetting>();
            var uri = setting.DefaultValue;

            var argsRoot = options.WorkspaceRoot;

            if (argsRoot is null && options.ClientProcessId == default && settings.RequireAddInHost)
            {
                _logger.LogDebug("An add-in host is required and should have provided a workspace root command-line argument. The current configuration does not support standalone execution.");
                throw new NotSupportedException("An add-in host is required and should have provided a workspace root command-line argument. The current configuration does not support standalone execution.");
            }
            else if (argsRoot is null)
            {
                // editor is running standalone without an addin client connection.
                _logger.LogDebug("No workspace root commad-line argument was specified, but configuration supports standalone execution. Using default workspace root; there is no project file or workspace folder yet.");
                return setting.DefaultValue;
            }

            if (Uri.TryCreate(argsRoot, UriKind.Absolute, out var argsRootUri))
            {
                uri = argsRootUri;
            }
            else
            {
                _logger.LogWarning("Could not parse value '{argsRoot}' into a valid URI. Falling back to default workspace root.", argsRoot);
            }

            if (settings.WorkspaceSettings.RequireDefaultWorkspaceRootHost && !uri.LocalPath.StartsWith(setting.DefaultValue.LocalPath))
            {
                _logger.LogWarning(TraceLevel, $"Configuration requires a workspace root under the default folder, but a folder under a different root was supplied.", uri.LocalPath);
                throw new NotSupportedException($"Configuration requires a workspace root under the default folder, but a folder under a different root was supplied.");
            }

            if (!settings.WorkspaceSettings.EnableUncWorkspaces && uri.IsUnc)
            {
                _logger.LogWarning(TraceLevel, $"UNC URI is not allowed: {nameof(settings.WorkspaceSettings.EnableUncWorkspaces)} setting is disabled. Default setting value will be used instead.", uri.ToString());
            }

            return uri;
        }
    }
}
