using Rubberduck.RPC.Properties;
using System;
using System.Diagnostics;
using System.Linq;

namespace Rubberduck.RPC
{
    /// <summary>
    /// A helper class to start a server process from a client.
    /// </summary>
    public static class ServerProcessClientHelper
    {
        /// <summary>
        /// Starts the <c>Rubberduck.Server.LocalDb</c> server if it isn't started already.
        /// </summary>
        /// <param name="port"></param>
        /// <returns>Returns the server process, whether it was already running or the method call started it.</returns>
        public static Process StartLocalDb(bool hidden = true)
        {
            var info = new ProcessStartInfo
            {
                FileName = Settings.Default.ServerExecutable_LocalDb,
                Arguments = $"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = hidden,
            };
            Process serverProcess;
            
            try
            {
                serverProcess = Process.Start(info);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                if (TryFindServerProcess(Settings.Default.ServerExecutable_LocalDb, out serverProcess))
                {
                    Debug.WriteLine($"Found existing '{serverProcess.ProcessName}' process (ID {serverProcess.Id}).");
                }
            }

            return serverProcess ?? throw new InvalidOperationException("Could not start or find a localdb server process.");
        }

        private static bool TryFindServerProcess(string name, out Process process)
        {
            process = Process.GetProcessesByName(name).FirstOrDefault();
            return process != null;
        }

        public static Process StartLSP()
        {
            var info = new ProcessStartInfo
            {
                FileName = Settings.Default.ServerExecutable_LSP,
                Arguments = $"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            return Process.Start(info);
        }

        public static Process StartTelemetry()
        {
            var info = new ProcessStartInfo
            {
                FileName = Settings.Default.ServerExecutable_Telemetry,
                Arguments = $"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            return Process.Start(info);
        }
    }
}