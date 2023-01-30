using CommandLine;
using System;
using System.Linq;
using System.Text;

namespace Rubberduck.Client.LocalDb
{
    public static partial class Program
    {
        public class StartupOptions
        {
            [Option('h', "hidden", HelpText = "Whether to hide the application's main window. UI may be shown later from the system tray.")]
            public bool Hidden { get; set; }

            public static StartupOptions Validate(string[] args)
            {
                var result = Parser.Default.ParseArguments<StartupOptions>(args)
                    .WithNotParsed(errors =>
                    {
                        using (var stdErr = Console.OpenStandardError())
                        {
                            var message = Encoding.UTF8.GetBytes("Errors have occurred processing command-line arguments. Server will not be started.");
                            stdErr.Write(message, 0, message.Length);
                        }
                    })
                    .WithParsed(options =>
                    {
                        using (var stdOut = Console.OpenStandardOutput())
                        {
                            var message = Encoding.UTF8.GetBytes("Command-line arguments successfully parsed. Proceeding with startup...");
                            stdOut.Write(message, 0, message.Length);
                        }
                    });

                if (result.Errors.Any())
                {
                    throw new ArgumentException("Invalid command-line arguments were supplied.");
                }

                return result.Value;
            }
        }
    }
}
