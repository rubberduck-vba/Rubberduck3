using Rubberduck.ServerPlatform;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.TelemetryServer
{
    public static class Program
    {
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            using var tokenSource = new CancellationTokenSource();
            var options = await ServerArgs.ParseAsync(args);

            try
            {
                await new ServerApp(options, tokenSource).RunAsync();
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}