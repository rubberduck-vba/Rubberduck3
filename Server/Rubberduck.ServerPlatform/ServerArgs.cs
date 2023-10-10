using CommandLine;
using Rubberduck.InternalApi.ServerPlatform;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubberduck.ServerPlatform
{
    public abstract class ServerStartupOptions
    {
        protected ServerStartupOptions(TransportType transportType)
        {
            TransportType = transportType;
        }

        public TransportType TransportType { get; }

        [Option('c', "client", Required = true, HelpText = "The process ID of the client that starts and owns this server.")]
        public int ClientProcessId { get; set; }

        [Option('v', "verbose", SetName = "TraceLevel", HelpText = "Whether to output verbose messages.")]
        public bool Verbose { get; set; }

        [Option('s', "silent", SetName = "TraceLevel", HelpText = "Whether or not to enable trace logging.")]
        public bool Silent { get; set; }


        /// <summary>
        /// Gets a string that corresponds to the <c>InitializeTrace</c> value for the specified <c>Verbose</c> and <c>Silent</c> switch arguments.
        /// </summary>
        public string TraceLevel => Silent ? "Off" : Verbose ? "Verbose" : "Messages";

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    [Verb("Pipe")]
    public class PipeServerStartupOptions : ServerStartupOptions
    {
        public PipeServerStartupOptions() : base(TransportType.Pipe) { }


        [Option('n', "Name", Default = ServerPlatformSettings.LanguageServerDefaultPipeName, HelpText = "The name of the transport pipe.")]
        public string Name { get; set; } = ServerPlatformSettings.LanguageServerDefaultPipeName;

        [Option('m', "Mode", Default = PipeTransmissionMode.Byte, HelpText = "The pipe's transmission mode. Use 'Message' for RPC-level trace debugging.")]
        public PipeTransmissionMode Mode { get; set; } = PipeTransmissionMode.Byte;

        /// <summary>
        /// The actual name of the pipe stream concatenates the <c>Name</c> with the <c>ClientProcessId</c> to ensure different hosts/instances use dedicated channels.
        /// </summary>
        public string PipeName => $"{Name}__{ClientProcessId}";
    }

    [Verb("StdIO", isDefault: true)]
    public class StandardInOutServerStartupOptions : ServerStartupOptions
    {
        public StandardInOutServerStartupOptions() : base(TransportType.StdIO) { }
    }

    public class ServerArgs
    {
        public static ServerStartupOptions Default { get; } = new StandardInOutServerStartupOptions();

        public static async Task<ServerStartupOptions> ParseAsync(string[] args)
        {
            if (args.Length == 0)
            {
                return ServerArgs.Default;
            }

            var parser = new Parser(ConfigureParser);
            var parserResult = parser.ParseArguments<StandardInOutServerStartupOptions, PipeServerStartupOptions>(args);
            
            if (parserResult.Errors.Any())
            {
                foreach (var error in parserResult.Errors)
                {
                    await Console.Error.WriteLineAsync($"Command-line argument parser error: '{error.Tag}'");
                }
                throw new ArgumentOutOfRangeException(nameof(args), "One or more errors have occurred parsing the specified command-line arguments. Process will be terminated.");
            }

            return (ServerStartupOptions)parserResult.Value;
        }

        private static void ConfigureParser(ParserSettings settings)
        {
            // BUG these settings appear to be ineffective, enum arg remains case-sensitive
            settings.CaseSensitive = false;
            settings.CaseInsensitiveEnumValues = false;
        }
    }
}