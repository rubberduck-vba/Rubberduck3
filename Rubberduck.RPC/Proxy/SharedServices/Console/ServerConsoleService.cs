using System;
using Rubberduck.RPC.Platform;
using System.Text;
using System.Threading;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Abstract
{
    public class ServerConsoleService<TOptions> : ServerProxyService<TOptions, ServerConsoleCommands>, IServerConsoleService<TOptions>
        where TOptions : SharedServerCapabilities, new()
    {
        private int _nextMessageId = 0;

        public ServerConsoleService(IServerStateService<TOptions> serverStateService)
            : base(null, serverStateService)
        {
            Logger = new ServerLogger(LogError, LogMessage);
        }

        public override ServerConsoleCommands Commands { get; }
        public override IServerLogger Logger { get; }

        public Type ClientProxyType { get; } = typeof(IServerConsoleProxyClient);

        private void LogError(Exception exception) => Log(exception);
        private void LogMessage(ServerLogLevel level, string message, string verbose) => Log(level, message, verbose);

        public event EventHandler<ConsoleMessage> Message;
        protected void OnMessage(int id, ServerLogLevel level, string message, string verbose, Exception exception = null)
            => Message?.Invoke(this, new ConsoleMessage(id, DateTime.Now, level, message, Configuration.ConsoleOptions.IsVerbose ? verbose : null, exception));

        public event EventHandler<LogTraceParams> LogTrace;
        protected void OnLogTrace(LogTraceParams parameters) => LogTrace?.Invoke(this, parameters);

        public void Log(ServerLogLevel level, string message, string verbose = null)
        {
            if (!Configuration.ConsoleOptions.CanLog(level))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "(no message)";
            }

            var id = Interlocked.Increment(ref _nextMessageId);
            var timestamp = DateTime.Now.ToString(Configuration.ConsoleOptions.ConsoleOutputFormatting.TimestampFormatString);

            var separator = Configuration.ConsoleOptions.ConsoleOutputFormatting.MessagePartSeparator;

            var willNotify = level != ServerLogLevel.Off && level >= Configuration.ConsoleOptions.LogLevel;
            var builder = willNotify ? new StringBuilder() : null;

            ConsoleOutput(builder, level, id.ToString(), LogOutputPart.MessageId);
            ConsoleOutput(builder, level, separator);

            ConsoleOutput(builder, level, timestamp, LogOutputPart.Timestamp);
            ConsoleOutput(builder, level, separator);

            ConsoleOutput(builder, level, level.ToString().ToUpperInvariant(), LogOutputPart.LogLevel);
            ConsoleOutput(builder, level, separator);

            ConsoleOutput(builder, level, message, LogOutputPart.Message);
            ConsoleOutput(builder, level, separator);

            if (Configuration.ConsoleOptions.Trace == Constants.Console.VerbosityOptions.AsStringEnum.Verbose && !string.IsNullOrWhiteSpace(verbose))
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
            var provider = Configuration.ConsoleOptions.ConsoleOutputFormatting;
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

        protected override void RpcClientSetTrace(object sender, SetTraceParams e)
        {
            _ = Task.Run(() => Commands.SetTraceCommand.ExecuteAsync(e));
        }

        protected override void RpcClientRequestExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void RpcClientInitialized(object sender, InitializedParams e)
        {
            throw new NotImplementedException();
        }
    }
}
