using System;

namespace Rubberduck.InternalApi.ServerPlatform
{
    public readonly struct InitializationOptions
    {
        public DateTime Timestamp { get; init; }
        public string Locale { get; init; }
        public string HostApplication { get; init; }
    }
}