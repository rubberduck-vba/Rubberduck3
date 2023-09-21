using Rubberduck.ServerPlatform;
using System;
using System.Threading.Tasks;
using System.Threading;

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
            using (var tokenSource = new CancellationTokenSource())
            {
                var options = await ServerArgs.ParseAsync(args);
                await new ServerApp(options).RunAsync();
            }
            return 0;
        }
    }
}