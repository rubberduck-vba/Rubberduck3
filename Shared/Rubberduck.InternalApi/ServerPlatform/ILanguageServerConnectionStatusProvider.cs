using System;

namespace Rubberduck.InternalApi.ServerPlatform
{
    public interface ILanguageServerConnectionStatusProvider
    {
        event EventHandler Connecting;
        event EventHandler Connected;
        event EventHandler Disconnected;
    }
}
