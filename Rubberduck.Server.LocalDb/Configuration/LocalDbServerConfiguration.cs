using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices.Console;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using System;

namespace Rubberduck.Server.LocalDb
{
    internal static class LocalDbServerConfiguration
    {
        /// <summary>
        /// Gets the default configuration for this server, taking the specified <see cref="StartupOptions"/> into account.
        /// </summary>
        public static LocalDbServerCapabilities Default(StartupOptions startupOptions)
        {
            return new LocalDbServerCapabilities
            {
                ConsoleOptions = new ServerConsoleOptions
                {
                    IsEnabled = !startupOptions.Silent,
                    Trace = startupOptions.Verbose 
                        ? Constants.Console.VerbosityOptions.AsStringEnum.Verbose 
                        : Constants.Console.VerbosityOptions.AsStringEnum.Message,
                    ConsoleOutputFormatting = new ConsoleOutputFormatOptions
                    {
                        MessagePartSeparator = " ",
                        FontFormatting = new FontFormattingOptions
                        {
                            DefaultFont = new FontOptions
                            {
                                FontFamily = "Consolas",
                                ForegroundColorProvider = new ConsoleColorOptions
                                {
                                    Default = ConsoleColor.Gray,
                                },
                            },
                        },
                        BackgroundFormatting = new BackgroundFormattingOptions
                        {
                            DefaultFormatProvider = new ConsoleColorOptions
                            {
                                Default = ConsoleColor.Gray,
                            }
                        }
                    }
                },
                
                /* TODO */
            };
        }
    }
}
