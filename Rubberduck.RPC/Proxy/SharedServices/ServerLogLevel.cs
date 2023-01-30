namespace Rubberduck.RPC.Proxy.SharedServices
{
    /// <summary>
    /// The log level of a message.
    /// </summary>
    /// <remarks>
    /// Values are consistent with <see cref="NLog.LogLevel"/> ordinals.
    /// </remarks>
    public enum ServerLogLevel
    {
        /// <summary>
        /// Trace level. Value matches <see cref="NLog.LogLevel.Trace.Ordinal"/>
        /// </summary>
        Trace = 0,
        /// <summary>
        /// Debug level. Value matches <see cref="NLog.LogLevel.Debug.Ordinal"/>
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Info level. Value matches <see cref="NLog.LogLevel.Info.Ordinal"/>
        /// </summary>
        Info = 2,
        /// <summary>
        /// Warn level. Value matches <see cref="NLog.LogLevel.Warn.Ordinal"/>
        /// </summary>
        Warn = 3,
        /// <summary>
        /// Error level. Value matches <see cref="NLog.LogLevel.Error.Ordinal"/>
        /// </summary>
        Error = 4,
        /// <summary>
        /// Fatal level. Value matches <see cref="NLog.LogLevel.Fatal.Ordinal"/>
        /// </summary>
        Fatal = 5,
        /// <summary>
        /// No logging. Value matches <see cref="NLog.LogLevel.Off.Ordinal"/>
        /// </summary>
        Off = 6
    }
}
