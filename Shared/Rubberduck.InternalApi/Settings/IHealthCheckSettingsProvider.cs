using System;

namespace Rubberduck.InternalApi.Settings;

public interface IHealthCheckSettingsProvider
{
    MessageTraceLevel ServerTraceLevel { get; }
    TimeSpan ClientHealthCheckInterval { get; }
}