using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Abstract
{
    public class ServerConsoleProxyService<TOptions> : ServerSideProxyService<TOptions>, IServerConsoleProxy<TOptions>
        where TOptions : SharedServerCapabilities, new()
    {
        private int _nextMessageId = 0;

        public ServerConsoleProxyService(IServerStateService<TOptions> serverStateService, IServerConsoleProxyClient client)
            : base(null, serverStateService)
        {
            Logger = new ServerLogger<TOptions>(this);

            client.SetTrace += Client_SetTrace;
            client.StopTrace += Client_StopTrace;
            client.ResumeTrace += Client_ResumeTrace;

            var getConfig = new GetServerOptions<ServerConsoleOptions>(() => Configuration);
            var getState = new GetServerStateInfo(() => ServerStateService.Info);

            _setEnabledCommand = new SetEnabledNotificationCommand(Logger, getConfig, getState);
            _setTraceCommand = new SetTraceNotificationCommand(Logger, getConfig, getState);
        }

        private readonly SetEnabledNotificationCommand _setEnabledCommand;
        private async void Client_ResumeTrace(object sender, EventArgs e)
        {
            await _setEnabledCommand.ExecuteAsync(new SetEnabledParams { Value = true });
        }

        private async void Client_StopTrace(object sender, EventArgs e)
        {
            await _setEnabledCommand.ExecuteAsync(new SetEnabledParams { Value = false });
        }

        private readonly SetTraceNotificationCommand _setTraceCommand;
        private async void Client_SetTrace(object sender, SetTraceParams e)
        {
            await _setTraceCommand.ExecuteAsync(e);
        }

        public override IServerLogger Logger { get; }

        public ServerConsoleOptions Configuration => ServerOptions.ConsoleOptions;

        public event EventHandler<LogTraceParams> LogTrace;
        public async Task OnLogTraceAsync(LogTraceParams parameters) => await Task.Run(() => LogTrace?.Invoke(this, parameters));

        public async Task LogTraceAsync(ServerLogLevel level, string message, string verbose = null)
        {
            if (!ServerOptions.ConsoleOptions.CanLog(level))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "(no message)";
            }

            var id = Interlocked.Increment(ref _nextMessageId);
            var timestamp = DateTime.Now.ToString(ServerOptions.ConsoleOptions.ConsoleOutputFormatting.TimestampFormatString);

            var separator = ServerOptions.ConsoleOptions.ConsoleOutputFormatting.MessagePartSeparator;

            var willNotify = level != ServerLogLevel.Off && level >= ServerOptions.ConsoleOptions.LogLevel;
            var builder = willNotify ? new StringBuilder() : null;

            await ConsoleOutputAsync(builder, level, id.ToString(), LogOutputPart.MessageId);
            await ConsoleOutputAsync(builder, level, separator);

            await ConsoleOutputAsync(builder, level, timestamp, LogOutputPart.Timestamp);
            await ConsoleOutputAsync(builder, level, separator);

            await ConsoleOutputAsync(builder, level, level.ToString().ToUpperInvariant(), LogOutputPart.LogLevel);
            await ConsoleOutputAsync(builder, level, separator);

            await ConsoleOutputAsync(builder, level, message, LogOutputPart.Message);
            await ConsoleOutputAsync(builder, level, separator);

            if (ServerOptions.ConsoleOptions.Trace == Constants.Console.VerbosityOptions.AsStringEnum.Verbose && !string.IsNullOrWhiteSpace(verbose))
            {
                await ConsoleOutputAsync(builder, level, verbose, LogOutputPart.Verbose);
            }

            await ConsoleOutputAsync(builder, level, Environment.NewLine);

            if (willNotify)
            {
                await OnLogTraceAsync(new LogTraceParams { Message = builder.ToString() });
            }
        }

        public async Task LogTraceAsync(Exception exception, ServerLogLevel level, string message = null, string verbose = null)
        {
            var logMessage = string.IsNullOrWhiteSpace(message) ? exception.Message : message;
            var logVerbose = string.IsNullOrWhiteSpace(verbose) ? exception.StackTrace : verbose;
            await LogTraceAsync(level, logMessage, logVerbose);
        }

        public async Task LogTraceAsync(Exception exception)
        {
            await LogTraceAsync(exception, ServerLogLevel.Error, null);
        }

        private async Task ConsoleOutputAsync(StringBuilder messageBuilder, ServerLogLevel level, string message, LogOutputPart? messagePart = null)
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
                await System.Console.Error.WriteAsync(message);
            }
            else
            {
                await System.Console.Out.WriteAsync(message);
            }
        }

        private void SetConsoleColors(LogOutputPart? part = null, ServerLogLevel? level = null)
        {
            var provider = ServerOptions.ConsoleOptions.ConsoleOutputFormatting;
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
