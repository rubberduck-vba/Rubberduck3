using Microsoft.Extensions.Logging;
using Rubberduck.Editor.RPC;
using Rubberduck.ServerPlatform;
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

        protected override ServerProcess GetServerProcess() => new LanguageServerProcess(_logger);
    }
}
