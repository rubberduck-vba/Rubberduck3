using Rubberduck.ServerPlatform;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace Rubberduck.LanguageServer
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            using var tokenSource = new CancellationTokenSource();
            var options = await ServerArgs.ParseAsync(args);
            if (options.Debug)
            {
                Debugger.Launch();
                Debug.Assert(Debugger.IsAttached, "Debugger is not attached.");
            }

            await new ServerApp(options, tokenSource).RunAsync();
            return 0;
        }
    }
}