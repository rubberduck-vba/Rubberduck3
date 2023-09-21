using Rubberduck.ServerPlatform;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

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
            Console.WriteLine(ServerSplash.GetRenderString(Assembly.GetExecutingAssembly().GetName()));

            using (var tokenSource = new CancellationTokenSource())
            {
                var options = await ServerArgs.ParseAsync(args);
                await new ServerApp(options).RunAsync();
            }
            return 0;
        }
    }
}