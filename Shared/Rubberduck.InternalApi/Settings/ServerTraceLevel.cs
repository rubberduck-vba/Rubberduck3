using System.Diagnostics;

namespace Rubberduck.InternalApi.Settings
{
    public enum ServerTraceLevel
    {
        Off = TraceLevel.Off,
        Verbose = TraceLevel.Verbose,
        Message = TraceLevel.Info,
    }
}
