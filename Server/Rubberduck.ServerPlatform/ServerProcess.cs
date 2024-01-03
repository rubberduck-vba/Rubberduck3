using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using System;
using System.Diagnostics;
using System.IO;

namespace Rubberduck.ServerPlatform
{
    public interface IServerProcess
    {
        Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings, EventHandler onExit);
    }

    public abstract class ServerProcess : IServerProcess
    {
        protected ILogger Logger { get; init; }

        protected ServerProcess(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract string ExecutableFileName { get; }

        public virtual Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings, EventHandler onExit)
        {
            var path = settings.ServerExecutablePath;
            var fullPath = Path.Combine(path, ExecutableFileName);

            if (!File.Exists(fullPath))
            {
                Logger.LogWarning(TraceLevel.Verbose,
                    $"{settings.GetType().Name}.ServerExecutablePath configuration is invalid.",
                    $"Configured value '{path}' should be a folder that contains the '{ExecutableFileName}' executable.");

                throw new FileNotFoundException($"ServerExecutablePath configuration is invalid.");
            }

            var info = new ProcessStartInfo
            {
                FileName = fullPath,
                WorkingDirectory = path,
                Arguments = settings.ToProcessStartInfoArguments(clientProcessId),
                CreateNoWindow = true,
                UseShellExecute = false,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            var process = new Process { StartInfo = info };
            process.Exited += onExit;

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