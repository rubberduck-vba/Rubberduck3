﻿using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using IOPath = System.IO.Path;

namespace Rubberduck.ServerPlatform
{
    public abstract class ServerProcess<TServer>
    {
        private readonly TServer _server;

        protected ServerProcess(IServiceProvider provider)
        {
            _server = provider.GetRequiredService<TServer>();
            
        }

        public abstract string Path { get; }

        protected abstract Task StartAsync(TServer server);

        public virtual async Task<int> StartAsync(bool hidden = true, string args = null)
        {
            if (TryFindServerProcess(IOPath.GetFileNameWithoutExtension(Path), out var process))
            {
                return process.Id;
            }
            else
            {
                var root = Directory.GetParent(Assembly.GetExecutingAssembly().Location)
                    .Parent // bin
                    .Parent // Rubberduck.Main
                    .Parent // Client
                    .Parent; // Rubberduck3

                var path = IOPath.Combine(root.FullName, @"Server\Rubberduck.LanguageServer\bin\Debug\net6.0", Path);
                var info = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = hidden,
                };

                process = Process.Start(info);
            }

            await StartAsync(_server);
            return process.Id;
        }

        protected bool TryFindServerProcess(string name, out Process process)
        {
            process = null;

            try
            {
                var matches = Process.GetProcessesByName(name);
                if (matches.Length > 0)
                {
                    Debug.Assert(matches.Length == 1);
                }
                process = matches[0];
                return process != null;
            }
            catch
            {
                return false;
            }
        }
    }
}