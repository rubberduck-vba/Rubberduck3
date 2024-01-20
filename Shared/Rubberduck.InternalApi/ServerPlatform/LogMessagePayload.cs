using NLog;
using Rubberduck.InternalApi.Extensions;
using System;
using System.Text.Json;

namespace Rubberduck.InternalApi.ServerPlatform
{
    public record class LogMessagePayload
    {
        public static LogMessagePayload FromLogEventInfo(LogEventInfo e)
        {
            var message = e.FormattedMessage;
            string? verbose = null;

            var i = message.IndexOf(LoggerExtensions.VerboseMessageSeparator);
            if (i > 0)
            {
                message = e.Message[..i].TrimEnd();
                verbose = e.Message[(i + 1)..].TrimStart();
            }

            string? exception = null;
            if (e.Exception != null)
            {
                exception = e.Exception.GetType().Name;
            }

            string? stackTrace = null;
            if (e.HasStackTrace)
            {
                stackTrace = e.StackTrace.ToString();
            }

            return new()
            {
                MessageId = e.SequenceID,
                Level = (Microsoft.Extensions.Logging.LogLevel)e.Level.Ordinal,
                Timestamp = e.TimeStamp,
                Message = message,
                Verbose = verbose,
                ExceptionType = exception,
                StackTrace = stackTrace,
            };
        }

        public int MessageId { get; init; }

        public Microsoft.Extensions.Logging.LogLevel Level { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public string Message { get; init; } = string.Empty;
        public string? Verbose { get; init; }

        public string? ExceptionType { get; init; }
        public string? StackTrace { get; init; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}