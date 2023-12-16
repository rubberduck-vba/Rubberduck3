using System;
using System.Diagnostics;

namespace Rubberduck.UI.LanguageServerTrace
{
    public interface ITraceMessageViewModel
    {
        DateTime Timestamp { get; }
        TraceLevel TraceLevel { get; }
        string Message { get; }
        string? Verbose { get; }
    }
}
