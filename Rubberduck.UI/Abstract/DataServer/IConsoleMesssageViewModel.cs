using NLog;
using Rubberduck.InternalApi.Common;
using System;

namespace Rubberduck.UI.Abstract
{
    public interface IConsoleMesssageViewModel : IExportable
    {

        int Id { get; }
        DateTime Timestamp { get; }
        /// <summary>
        /// Typically <c>true</c> if the <c>Exception</c> property contains error information.
        /// </summary>
        bool IsError { get; }
        /// <summary>
        /// Indicates an attention-worthy message.
        /// </summary>
        bool IsWarning { get; }
        /// <summary>
        /// The message level. Client may use it to filter messages displayed.
        /// </summary>
        LogLevel Level { get; }
        /// <summary>
        /// The message text.
        /// </summary>
        string Message { get; }
        /// <summary>
        /// Additional information shown when verbose tracing is enabled.
        /// </summary>
        string Verbose { get; }
        /// <summary>
        /// An exception with additional error information. <c>null</c> if there was no error.
        /// </summary>
        Exception Exception { get; }
    }
}
