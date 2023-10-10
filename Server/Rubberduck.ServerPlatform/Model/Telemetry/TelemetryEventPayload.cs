using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.ServerPlatform.Model.Telemetry
{
    public record TelemetryEventPayload : TelemetryEventParams
    {
        protected TelemetryEventPayload(TelemetryEventParams original) 
            : base(original) { }


    }
}
