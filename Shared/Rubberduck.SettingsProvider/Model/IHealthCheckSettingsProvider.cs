using System;

namespace Rubberduck.SettingsProvider.Model
{
    public interface IHealthCheckSettingsProvider
    {
        ServerTraceLevel TraceLevel { get; }
        TimeSpan ClientHealthCheckInterval { get; }
    }
}