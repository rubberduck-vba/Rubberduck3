using System;

namespace Rubberduck.SettingsProvider.Model
{
    /// <summary>
    /// Marks a setting holding a sensitive value that should not be included in telemetry payloads.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TelemetrySensitiveAttribute : Attribute { }
}
