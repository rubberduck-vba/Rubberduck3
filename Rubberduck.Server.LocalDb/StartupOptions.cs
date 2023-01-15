using CommandLine;

namespace Rubberduck.Server.LocalDb
{
    internal class StartupOptions
    {
        [Option('p', "port", Required = true, HelpText = "Sets the RPC port for this server (required).")]
        public int Port { get; set; }

        [Option('i', "interactive", Required = false, HelpText = "Whether this server should display a UI.")]
        public bool Interactive { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Whether this server logs verbose messages.")]
        public bool Verbose { get; set; }
        [Option('s', "silent", Required = false, HelpText = "Whether trace output is disabled on this server.")]
        public bool Silent { get; set; }

        [Option('d', "exit-delay", Required = false, Default = 2000, HelpText = "The number of milliseconds to wait before exiting the process after shutting down.")]
        public int ExitDelayMilliseconds { get; set; }
    }
}
