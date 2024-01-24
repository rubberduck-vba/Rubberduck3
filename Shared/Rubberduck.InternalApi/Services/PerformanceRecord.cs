using System;

namespace Rubberduck.InternalApi.Services
{
    public record class PerformanceRecord
    {
        public string Name { get; init; }
        public TimeSpan Elapsed { get; init; }
    }
}
