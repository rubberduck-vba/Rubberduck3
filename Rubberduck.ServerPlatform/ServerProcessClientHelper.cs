using System.Diagnostics;
using System.Reflection;

namespace Rubberduck.ServerPlatform
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
            Process serverProcess = null;
            Exception originalException;
            try
            {
                if (TryFindServerProcess(ServerPlatform.Settings.DatabaseServerExecutableLocation, out serverProcess))
                {
                    Debug.WriteLine($"Found existing '{serverProcess.ProcessName}' process (ID {serverProcess.Id}).");
                    return serverProcess;
                }

                var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("EntryAssembly is unexpectedly null");
                var info = new ProcessStartInfo
                {
                    FileName = ServerPlatform.Settings.DatabaseServerExecutableLocation,
                    Arguments = $"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = hidden,
                    WorkingDirectory = Path.GetDirectoryName(assembly.Location),
                };

                return Process.Start(info);
            }
            catch (Exception exception)
            {
                originalException = exception;
            }

            return serverProcess ?? throw new InvalidOperationException("Could not start or find a localdb server process.", originalException);
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
                FileName = ServerPlatform.Settings.LanguageServerExecutableLocation,
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
                FileName = ServerPlatform.Settings.TelemetryServerExecutableLocation,
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