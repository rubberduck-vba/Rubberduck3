using System;

namespace Rubberduck.SettingsProvider
{
    public record class PerformanceRecord
    {
        public string Name { get; init; }
        public TimeSpan Elapsed { get; init; }
    }
}
