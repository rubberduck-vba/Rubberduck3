using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rubberduck.InternalApi.Settings.Model;

public static class TraceLevelExtensions
{
    private static readonly IDictionary<MessageTraceLevel, TraceLevel> _map = new Dictionary<MessageTraceLevel, TraceLevel>
    {
        [MessageTraceLevel.Off] = TraceLevel.Off,
        [MessageTraceLevel.Verbose] = TraceLevel.Verbose,
        [MessageTraceLevel.Message] = TraceLevel.Info,
    };

    public static TraceLevel ToTraceLevel(this MessageTraceLevel value) => _map[value];
}
