using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Rubberduck.Client
{
    public interface IServerProcess
    {
        Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings);
    }

    public abstract class ServerProcess<TServer> : IServerProcess
    {
        protected ILogger Logger { get; init; }

        protected ServerProcess(ILogger logger)
        {
            Logger = logger;
        }

        protected virtual string GetRelativePath(IProcessStartInfoArgumentProvider settings) => settings.Path;
        protected abstract string ExecutableFileName { get; }

        public virtual Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings)
        {
            var root = Directory.GetParent(Assembly.GetExecutingAssembly().Location)!
#if DEBUG
                .Parent! // net7.0
                .Parent! // Debug
                .Parent! // bin
                .Parent! // Rubberduck.Main
                .Parent! // Client
                .Parent! // Rubberduck3
#endif
            ;
            var path = Path.Combine(root!.FullName
#if DEBUG
                , @"Server\Rubberduck.LanguageServer\bin\Debug\net7.0\win-x64"
#endif
            );

            var filename = Path.Combine(path, ExecutableFileName);
            var info = new ProcessStartInfo
            {
                FileName = filename,
                WorkingDirectory = Path.GetDirectoryName(filename),
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
                Logger.LogWarning(exception, "Could not start server process.", $"FileName: '{filename}'");
                throw;
            }
            return process;
        }
    }
}