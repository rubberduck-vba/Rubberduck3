using Microsoft.Extensions.Logging;
using Rubberduck.Editor.RPC;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Threading;

namespace Rubberduck.Editor
{
    /// <summary>
    /// Configures LSP for editor <--> language server communications.
    /// </summary>
    public sealed class LanguageClientApp : RubberduckClientApp
    {
        private readonly ILogger _logger;
        public LanguageClientApp(ILogger<LanguageClientApp> logger, ServerStartupOptions options, CancellationTokenSource tokenSource, IServiceProvider services)
            : base(logger, options, tokenSource, services)
        {
            _logger = logger;
        }

        protected override ServerStartupSettings GetStartupOptions(RubberduckSettings settings) => settings.LanguageServerSettings.StartupSettings;
        protected override RubberduckServerProcess GetServerProcess() => new LanguageServerProcess(_logger);
    }
}
