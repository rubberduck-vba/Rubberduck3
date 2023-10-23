using System;

namespace Rubberduck.InternalApi.Settings
{
    public interface IHealthCheckSettingsProvider
    {
        ServerTraceLevel ServerTraceLevel { get; }
        TimeSpan ClientHealthCheckInterval { get; }
    }
}