using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rubberduck.SettingsProvider.Model
{
    public static class TraceLevelExtensions
    {
        private static readonly IDictionary<ServerTraceLevel, TraceLevel> _map = new Dictionary<ServerTraceLevel, TraceLevel>
        {
            [ServerTraceLevel.Off] = TraceLevel.Off,
            [ServerTraceLevel.Verbose] = TraceLevel.Verbose,
            [ServerTraceLevel.Message] = TraceLevel.Info,
        };

        public static TraceLevel ToTraceLevel(this ServerTraceLevel value) => _map[value];
    }
}
