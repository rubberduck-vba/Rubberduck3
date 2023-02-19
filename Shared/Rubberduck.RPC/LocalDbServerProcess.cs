using System.Diagnostics;

namespace Rubberduck.RPC
{
    public static class LocalDbServerProcess
    {
        public static void Start(int port, bool interactive)
        {
            Process.Start($"Rubberduck.Server.LocalDb.exe -p {port}{(interactive ? " -i" : string.Empty)}");
        }
    }
}