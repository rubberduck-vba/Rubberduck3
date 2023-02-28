using System.Diagnostics;

namespace Rubberduck.RPC
{
    public static class LspServerProcess
    {
        public static void Start(int port, bool interactive)
        {
            Process.Start($"Rubberduck.Server.LSP.exe -p {port}{(interactive ? " -i" : string.Empty)}");
        }
    }
}