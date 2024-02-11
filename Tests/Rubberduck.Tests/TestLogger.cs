using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System;

namespace Rubberduck.Tests
{
    internal class TestLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (exception is not null)
            {
                Logger.LogMessage(formatter.Invoke(state, exception));
            }
            else if (state is string message)
            {
                Logger.LogMessage("{0}: {1}", logLevel.ToString().ToUpperInvariant(), message);
            }
        }
    }
}
