using System;

namespace Rubberduck.InternalApi.Settings.Model;

/// <summary>
/// Marks a setting holding a sensitive value that should not be included in telemetry payloads.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TelemetrySensitiveAttribute : Attribute { }
