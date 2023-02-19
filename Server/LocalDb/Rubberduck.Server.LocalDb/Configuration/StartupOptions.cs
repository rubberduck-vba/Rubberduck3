using CommandLine;

namespace Rubberduck.Server.LocalDb.Configuration
{
    /// <summary>
    /// The server startup options, defined by command-line arguments.
    /// </summary>
    public class StartupOptions
    {
        /// <summary>
        /// Whether this server starts with verbose messages enabled.
        /// </summary>
        [Option('v', "verbose", Required = false, HelpText = "Whether this server starts with verbose messages enabled.")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Whether trace output is disabled on this server.
        /// </summary>
        [Option('s', "silent", Required = false, HelpText = "Whether trace output is disabled on this server.")]
        public bool Silent { get; set; }
    }
}
