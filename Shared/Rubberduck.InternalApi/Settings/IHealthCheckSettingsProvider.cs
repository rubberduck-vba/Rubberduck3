using System;

namespace Rubberduck.InternalApi.Settings
{
    public interface IHealthCheckSettingsProvider
    {
        ServerTraceLevel TraceLevel { get; }
        TimeSpan ClientHealthCheckInterval { get; }
    }
}