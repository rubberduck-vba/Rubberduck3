using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.ServerPlatform
{
    public abstract class ServerProcess<TServer>
    {
        protected virtual string RelativePath { get; } = string.Empty;
        protected abstract string ExecutableFileName { get; }

        protected abstract Task InitializeAsync(TServer server, CancellationToken token);

        public virtual Process Start(bool hidden = true, string args = null)
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
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                //RedirectStandardError = true,
                CreateNoWindow = hidden,
            };

            var process = Process.Start(info);
            return process;
        }
    }
}