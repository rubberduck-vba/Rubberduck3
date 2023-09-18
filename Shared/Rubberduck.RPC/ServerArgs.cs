using CommandLine;

namespace Rubberduck.ServerPlatform
{
    public abstract class TransportOptions
    {
        protected TransportOptions(TransportType transportType)
        {
            TransportType = transportType;
        }

        public TransportType TransportType { get; }


        [Option('v', "verbose", HelpText = "Whether to output verbose messages.")]
        public bool Verbose { get; set; }
    }

    [Verb("Pipe")]
    public class PipeTransportOptions : TransportOptions
    {
        public PipeTransportOptions() : base(TransportType.Pipe) { }

        [Option('n', "Name", Default = "Rubberduck.LSP.Pipe", HelpText = "The name of the transport pipe.")]
        public string Name { get; set; }

        [Option('c', "client", Required = true, HelpText = "The process ID of the client that starts and owns this server.")]
        public int ClientProcessId { get; set; }

        public string PipeName => $"{Name}__{ClientProcessId}";
    }

    [Verb("StdIO", isDefault: true)]
    public class StandardInOutTransportOptions : TransportOptions
    {
        public StandardInOutTransportOptions() : base(TransportType.StdIO) { }
    }

    public class ServerArgs
    {
        public static TransportOptions Default { get; } = new StandardInOutTransportOptions();

        public static async Task<TransportOptions> ParseAsync(string[] args)
        {
            var parser = new Parser(ConfigureParser);
            var parserResult = parser.ParseArguments<StandardInOutTransportOptions, PipeTransportOptions>(args);
            
            if (parserResult.Errors.Any())
            {
                foreach (var error in parserResult.Errors)
                {
                    await Console.Error.WriteLineAsync($"Command-line argument parser error: '{error.Tag}'");
                }
                throw new ArgumentOutOfRangeException(nameof(args), "One or more errors have occurred parsing the specified command-line arguments. Process will be terminated.");
            }

            return parserResult.Value as TransportOptions;
        }

        private static void ConfigureParser(ParserSettings settings)
        {
            // BUG these settings appear to be ineffective, enum arg remains case-sensitive
            settings.CaseSensitive = false;
            settings.CaseInsensitiveEnumValues = false;
        }
    }
}