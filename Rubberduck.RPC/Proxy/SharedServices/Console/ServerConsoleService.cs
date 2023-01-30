using System;
using System.Text;
using System.Threading;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;

namespace Rubberduck.RPC.Proxy.SharedServices.Console
{
    public class ServerConsoleService<TOptions> : ServerProxyService<TOptions, ServerConsoleCommands>, IServerConsoleService<TOptions>
        where TOptions : ServerConsoleOptions, new()
    {
        private int _nextMessageId = 0;

        public ServerConsoleService(TOptions configuration, GetServerStateInfo getServerState)
            : base(null, () => configuration, getServerState)
        {
            GetServerOptions<TOptions> getConfiguration = () => Configuration;
            Logger = new ServerLogger(LogError, LogMessage);
            
            var setTraceCommand = new SetTraceCommand(Logger, getConfiguration, getServerState);
            var setEnabledCommand = new SetEnabledCommand(Logger, getConfiguration, getServerState);
            var getOptionsCommand = new GetConsoleOptionsCommand(Logger, getConfiguration, getServerState);

            Commands = new ServerConsoleCommands(setTraceCommand, setEnabledCommand, getOptionsCommand);
        }

        public override ServerConsoleCommands Commands { get; }
        public override IServerLogger Logger { get; }

        private void LogError(Exception exception) => Log(exception);
        private void LogMessage(ServerLogLevel level, string message, string verbose) => Log(level, message, verbose);

        public event EventHandler<ConsoleMessage> Message;
        protected void OnMessage(int id, ServerLogLevel level, string message, string verbose, Exception exception = null)
            => Message?.Invoke(this, new ConsoleMessage(id, DateTime.Now, level, message, Configuration.IsVerbose ? verbose : null, exception));

        public event EventHandler<LogTraceParams> LogTrace;
        protected void OnLogTrace(LogTraceParams parameters) => LogTrace?.Invoke(this, parameters);

        public void Log(ServerLogLevel level, string message, string verbose = null)
        {
            if (!Configuration.CanLog(level))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "(no message)";
            }

            var id = Interlocked.Increment(ref _nextMessageId);
            var timestamp = DateTime.Now.ToString(Configuration.ConsoleOutputFormatting.TimestampFormatString);

            var separator = Configuration.ConsoleOutputFormatting.MessagePartSeparator;

            var willNotify = level != ServerLogLevel.Off && level >= Configuration.LogLevel;
            var builder =  willNotify ? new StringBuilder() : null;

            ConsoleOutput(builder, level, id.ToString(), LogOutputPart.MessageId);
            ConsoleOutput(builder, level, separator);

            ConsoleOutput(builder, level, timestamp, LogOutputPart.Timestamp);
            ConsoleOutput(builder, level, separator);

            ConsoleOutput(builder, level, level.ToString().ToUpperInvariant(), LogOutputPart.LogLevel);
            ConsoleOutput(builder, level, separator);

            ConsoleOutput(builder, level, message, LogOutputPart.Message);
            ConsoleOutput(builder, level, separator);

            if (Configuration.Trace == Constants.Console.VerbosityOptions.AsStringEnum.Verbose && !string.IsNullOrWhiteSpace(verbose))
            {
                ConsoleOutput(builder, level, verbose, LogOutputPart.Verbose);
            }

            ConsoleOutput(builder, level, Environment.NewLine);

            if (willNotify)
            {
                OnLogTrace(new LogTraceParams { Message = builder.ToString() });
            }

            OnMessage(id, level, message, verbose);
        }

        public void Log(Exception exception, ServerLogLevel level, string message = null, string verbose = null)
        {
            var logMessage = string.IsNullOrWhiteSpace(message) ? exception.Message : message;
            var logVerbose = string.IsNullOrWhiteSpace(verbose) ? exception.StackTrace : verbose;
            Log(level, logMessage, logVerbose);
        }

        public void Log(Exception exception)
        {
            Log(exception, ServerLogLevel.Error, null);
        }

        private void ConsoleOutput(StringBuilder messageBuilder, ServerLogLevel level, string message, LogOutputPart? messagePart = null)
        {
            messageBuilder?.Append(message);

            if (messagePart.HasValue)
            {
                SetConsoleColors(messagePart.Value, level);
            }
            else
            {
                SetConsoleColors();
            }

            if (level == ServerLogLevel.Error || level == ServerLogLevel.Fatal)
            {
                System.Console.Error.Write(message);
            }
            else
            {
                System.Console.Out.Write(message);
            }
        }

        private void SetConsoleColors(LogOutputPart? part = null, ServerLogLevel? level = null)
        {
            var provider = Configuration.ConsoleOutputFormatting;
            (Func<ConsoleColorOptions> foreground, Func<ConsoleColorOptions> background) config =
                (() => provider.FontFormatting.DefaultFont.ForegroundColorProvider, () => provider.BackgroundFormatting.DefaultFormatProvider);

            (ConsoleColor foreground, ConsoleColor background) = 
                (provider.FontFormatting.DefaultFont.ForegroundColorProvider.Default, provider.BackgroundFormatting.DefaultFormatProvider.Default);

            if (part.HasValue)
            {
                switch (part)
                {
                    case LogOutputPart.MessageId:
                        config = (() => provider.FontFormatting.MessageIdFont.ForegroundColorProvider, () => provider.BackgroundFormatting.IdBackgroundProvider);
                        break;
                    case LogOutputPart.Timestamp:
                        config = (() => provider.FontFormatting.TimestampFont.ForegroundColorProvider, () => provider.BackgroundFormatting.TimestampBackgroundProvider);
                        break;
                    case LogOutputPart.LogLevel:
                        config = (() => provider.FontFormatting.LogLevelFont.ForegroundColorProvider, () => provider.BackgroundFormatting.LogLevelBackgroundProvider);
                        break;
                    case LogOutputPart.Message:
                        config = (() => provider.FontFormatting.MessageFont.ForegroundColorProvider, () => provider.BackgroundFormatting.MessageBackgroundProvider);
                        break;
                    case LogOutputPart.Verbose:
                        config = (() => provider.FontFormatting.VerboseFont.ForegroundColorProvider, () => provider.BackgroundFormatting.VerboseBackgroundProvider);
                        break;
                    default:
                        throw new NotSupportedException($"LogOutputPart value {(int)part} is not supported.");
                }

                var foregroundProvider = config.foreground.Invoke();
                var backgroundProvider = config.background.Invoke();

                if (level.HasValue)
                {
                    foreground = foregroundProvider.For(level.Value);
                    background = backgroundProvider.For(level.Value);
                }
                else
                {
                    foreground = foregroundProvider.Default;
                    background = backgroundProvider.Default;
                }
            }

            System.Console.ForegroundColor = foreground;
            System.Console.BackgroundColor = background;
        }
    }
}
