using System;

namespace Rubberduck.InternalApi.ServerPlatform
{
    public readonly struct InitializationOptions
    {
        public DateTime Timestamp { get; init; }
        public long ClientProcessId { get; init; }
    }
}