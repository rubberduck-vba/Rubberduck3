using System.Diagnostics;

namespace Rubberduck.ServerPlatform
{
    public static class LanguageServerProcess
    {
        public static void Start(int port, bool interactive)
        {
            Process.Start($"Rubberduck.LanguageServer.exe -p {port}{(interactive ? " -i" : string.Empty)}");
        }
    }
}