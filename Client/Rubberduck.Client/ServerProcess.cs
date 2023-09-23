﻿using Rubberduck.SettingsProvider.Model;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Rubberduck.Client
{
    public interface IServerProcess
    {
        Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings);
    }

    public abstract class ServerProcess<TServer> : IServerProcess
    {
        protected virtual string GetRelativePath(IProcessStartInfoArgumentProvider settings) => settings.Path;
        protected abstract string ExecutableFileName { get; }

        public virtual Process Start(long clientProcessId, IProcessStartInfoArgumentProvider settings)
        {
            var root = Directory.GetParent(Assembly.GetExecutingAssembly().Location)
#if DEBUG
                .Parent // bin
                .Parent // Rubberduck.Main
                .Parent // Client
                .Parent // Rubberduck3
#endif
            ;
            var path = Path.Combine(root.FullName, settings.Path
#if DEBUG
                , @"bin\Debug\net6.0"
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
            process.Start();
            
            return process;
        }
    }
}