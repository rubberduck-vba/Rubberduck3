using Rubberduck.RPC.Properties;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rubberduck.RPC
{
    /// <summary>
    /// A helper class to start a server process from a client.
    /// </summary>
    public static class ServerProcessClientHelper
    {
        /// <summary>
        /// Starts the <c>Rubberduck.Server.Database</c> server if it isn't started already.
        /// </summary>
        /// <param name="port"></param>
        /// <returns>Returns the server process, whether it was already running or the method call started it.</returns>
        public static Process StartDatabase(bool hidden = true)
        {
            Process serverProcess = null;
            try
            {
                if (TryFindServerProcess(Settings.Default.ServerExecutable_Database, out serverProcess))
                {
                    Debug.WriteLine($"Found existing '{serverProcess.ProcessName}' process (ID {serverProcess.Id}).");
                    return serverProcess;
                }

                var info = new ProcessStartInfo
                {
                    FileName = Settings.Default.ServerExecutable_Database,
                    Arguments = $"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = hidden,
                    WorkingDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                };

                return Process.Start(info);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return serverProcess ?? throw new InvalidOperationException("Could not start or find a Database server process.");
        }

        private static bool TryFindServerProcess(string name, out Process process)
        {
            process = null;

            try
            {
                var matches = Process.GetProcessesByName(name);
                if (matches.Length > 0)
                {
                    Debug.Assert(matches.Length == 1);
                    process = matches[0];
                }
                return process != null;
            }
            catch
            {
                return false;
            }
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