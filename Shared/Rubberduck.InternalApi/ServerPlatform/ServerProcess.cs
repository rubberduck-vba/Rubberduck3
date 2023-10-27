using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Rubberduck.InternalApi.ServerPlatform
{
    public interface IServerProcess
    {
        Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings);
    }

    public abstract class ServerProcess : IServerProcess
    {
        protected ILogger Logger { get; init; }

        protected ServerProcess(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract string ExecutableFileName { get; }

        public virtual Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings)
        {
            var path = settings.ServerExecutablePath;
            var filename = Path.GetFileName(path);
            if (!string.Equals(filename, ExecutableFileName, StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.LogWarning(TraceLevel.Verbose, $"ServerExecutablePath configured filename is unexpected.", $"expected: '{ExecutableFileName}' actual: '{filename}'");
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"ServerExecutablePath configuration is invalid.");
            }

            var info = new ProcessStartInfo
            {
                FileName = path,
                WorkingDirectory = Path.GetDirectoryName(path),
                Arguments = settings.ToProcessStartInfoArguments(clientProcessId),
                CreateNoWindow = true,
                UseShellExecute = false,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            var process = new Process { StartInfo = info };
            try
            {
                process.Start();
                Logger.LogInformation("Server process started {start} with ID {id}", process.StartTime, process.Id);
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, "Could not start server process.", $"FileName: '{path}'");
                throw;
            }
            return process;
        }
    }
}