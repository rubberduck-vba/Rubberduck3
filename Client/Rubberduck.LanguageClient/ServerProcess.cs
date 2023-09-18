using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IOPath = System.IO.Path;

namespace Rubberduck.LanguageClient
{
    public abstract class ServerProcess<TServer>
    {
        protected virtual string RelativePath { get; } = string.Empty;
        protected abstract string ExecutableFileName { get; }

        protected abstract Task InitializeAsync(TServer server, CancellationToken token);

        public virtual Process Start(bool hidden = true, string args = null)
        {
           // if (TryFindServerProcess(IOPath.GetFileNameWithoutExtension(ExecutableFileName), out var process))
           // {
           //     return process;
           // }
           // else
           //{
                var root = Directory.GetParent(Assembly.GetExecutingAssembly().Location)
#if DEBUG
                    .Parent // bin
                    .Parent // Rubberduck.Main
                    .Parent // Client
                    .Parent // Rubberduck3
#endif
                ;
                var path = IOPath.Combine(root.FullName, RelativePath
#if DEBUG
                    , @"bin\Debug\net6.0"
#endif
                );

                var filename = IOPath.Combine(path, ExecutableFileName);
                var info = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = hidden,
                };

                return Process.Start(info);
            //}

            //return process;
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