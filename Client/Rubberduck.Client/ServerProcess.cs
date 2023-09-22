using Rubberduck.InternalApi.ServerPlatform;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Client
{
    public interface IServerProcess
    {
        Process Start(TransportType transport, bool hidden = true, string args = null);
    }

    public abstract class ServerProcess<TServer> : IServerProcess
    {
        protected virtual string RelativePath { get; } = string.Empty;
        protected abstract string ExecutableFileName { get; }

        public virtual Process Start(TransportType transport, bool hidden = true, string args = null)
        {
            var root = Directory.GetParent(Assembly.GetExecutingAssembly().Location)
#if DEBUG
                .Parent // bin
                .Parent // Rubberduck.Main
                .Parent // Client
                .Parent // Rubberduck3
#endif
            ;
            var path = Path.Combine(root.FullName, RelativePath
#if DEBUG
                , @"bin\Debug\net6.0"
#endif
            );

            var filename = Path.Combine(path, ExecutableFileName);
            var info = new ProcessStartInfo
            {
                FileName = filename,
                WorkingDirectory = Path.GetDirectoryName(filename),
                Arguments = args,
                CreateNoWindow = hidden,
                UseShellExecute = false,

                // conditional on TransportType.StdIO?
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            var process = new Process { StartInfo = info };
            process.Start();
            
            return process;
        }
    }
}