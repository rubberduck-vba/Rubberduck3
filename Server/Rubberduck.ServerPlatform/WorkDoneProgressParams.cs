using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.ServerPlatform
{
    public record class WorkDoneProgressParams : IWorkDoneProgressParams
    {
        public ProgressToken? WorkDoneToken { get; init; }
    }
}