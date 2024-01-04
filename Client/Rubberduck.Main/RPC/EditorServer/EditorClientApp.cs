using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform;
using System;
using System.Threading;

namespace Rubberduck.Main.RPC.EditorServer
{
    public sealed class EditorClientApp : RubberduckClientApp
    {
        private readonly ILogger _logger;
        public EditorClientApp(ILogger logger, IServiceProvider services, ServerStartupOptions options, CancellationTokenSource tokenSource)
            : base(logger, options, tokenSource, services)
        {
            _logger = logger;
        }

        protected override RubberduckServerProcess GetServerProcess() => new EditorServerProcess(_logger);
    }
}