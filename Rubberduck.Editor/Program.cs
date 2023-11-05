using Microsoft.Extensions.DependencyInjection;
using Rubberduck.ServerPlatform;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor
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

                var app = new App();
                return app.Run();
            }
            catch
            {
                return -1;
            }
        }
    }
}
