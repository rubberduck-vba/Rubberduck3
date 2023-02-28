using System;
using System.Diagnostics;
using System.Reflection;

namespace Rubberduck.Client.LocalDb
{
    public static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var options = StartupOptions.Validate(args);
            var process = GetOrCreateServer(options);

            process.OutputDataReceived -= OnServerProcessStdOut;
            process.ErrorDataReceived -= OnServerProcessStdErr;
            process.Exited -= OnServerProcessExit;

            var assembly = Assembly.GetExecutingAssembly().GetName();
            var clientProcess = Process.GetCurrentProcess();
            
            
        }

        private static Process GetOrCreateServer(StartupOptions startupOptions)
        {
            var process = RPC.ServerProcessClientHelper.StartLocalDb(hidden: false);
            
            process.OutputDataReceived += OnServerProcessStdOut;
            process.ErrorDataReceived += OnServerProcessStdErr;
            process.Exited += OnServerProcessExit;
            
            return process;
        }

        private static void OnServerProcessExit(object sender, EventArgs e)
        {
            Debug.WriteLine("Rubberduck.Server.LocalDb process has exited unexpectedly.");
        }

        private static void OnServerProcessStdErr(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        private static void OnServerProcessStdOut(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }
    }
}
