using Microsoft.Extensions.Logging;

namespace Rubberduck.InternalApi.Extensions
{
    /// <summary>
    /// A service responsible for setting loggers' minimum log level.
    /// </summary>
    public interface ILogLevelService
    {
        /// <summary>
        /// Sets the minimum <c>LogLevel</c> that actually writes to log targets.
        /// </summary>
        void SetMinimumLogLevel(LogLevel level);
    }
}
