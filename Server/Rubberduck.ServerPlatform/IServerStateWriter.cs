using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.ServerPlatform
{
    public interface IServerStateWriter
    {
        void Initialize(InitializeParams param);
        void Shutdown(ShutdownParams param);
        void SetTraceLevel(InitializeTrace level);
    }
}